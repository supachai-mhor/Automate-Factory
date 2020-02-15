using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AutomateBussiness.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using AutomateBussiness.Models;
using System.Security.Claims;

namespace AutomateBussiness.Hubs
{

    //[Authorize(Roles ="Machine")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub:Hub
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly AutomateBussinessContext _context;

        public ChatHub(AutomateBussinessContext context, UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
        }
        public class UserList
        {
            public string id;
            public string email;
            public string factoryName;
            public string machineName;
            public string role;
        }
        #region---Data Members---
        static List<UserList> listUserID = new List<UserList>();
        #endregion

        public override async Task OnConnectedAsync()
        {
            // var facName = userManager.Users.Where(m => m.UserName == Context.UserIdentifier).First().FactoryName;

            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                   .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();
            var roleType= claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == facName);
            if (factory.Count() > 0 && mcName != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, facName);
                var currentUser = new UserList
                {
                    id = Context.ConnectionId,
                    email = emailAddress,
                    factoryName = facName,
                    machineName = mcName,
                    role = roleType
                };
                if(mcName != "Viewer")
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ServerRequestRealTime", true, 60);
                    await Clients.Group(facName).SendAsync("ReceiveUserOnline", mcName);
                }
                else
                {
                    await Clients.Group(facName).SendAsync("ReceiveUserOnline", emailAddress);
                }

                
                listUserID.Add(currentUser);
            }
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                   .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == facName);

            if (factory.Count() > 0 && mcName != null)
            {
                if (mcName != "Viewer")
                {
                    await Clients.Group(facName).SendAsync("ReceiveStatusData", -2, mcName);
                    await Clients.Group(facName).SendAsync("ReceiveUserOffline", mcName);
                }
                else
                {
                     await Clients.Group(facName).SendAsync("ReceiveUserOffline", emailAddress);
                }
               
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, facName);
                var indexUser = listUserID.FindIndex(x => x.id == Context.ConnectionId);
                listUserID.RemoveAt(indexUser);
            }
            
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string user, string message)
        {
            var claims = Context.User.Claims;
            var groupName = claims.Where(c => c.Type == "FactoryName")
                   .Select(c => c.Value).SingleOrDefault();
            if(groupName != null)
            {
                var movie = await _context.Movie
                    .FirstOrDefaultAsync(m => m.Id == System.Convert.ToInt32(user));

                if (movie != null)
                {
                    string json = JsonConvert.SerializeObject(movie);
                    //await Clients.All.SendAsync("ReceiveData", movie.Price, json);
                    await Clients.Group(groupName).SendAsync("ReceiveData", movie.Price.ToString(), json);
                    await Clients.Group(groupName).SendAsync("ReceiveMessage", decimal.ToDouble(movie.Price), json);
                }
            }
            
            //else
            //{
            //    await Clients.All.SendAsync("ReceiveMessage", user, message);
            //}
            //await Clients.All.SendAsync("ReceiveData", user, message);

        }
        public async Task GetJobDetail(string jobName)
        {
            var planingViewModel = new PlaningViewModel
            {
                job_number = jobName,
                planQty = 20000,
                expectRatio = 98,
                qtyPerInput = 100,
            };


            string json = JsonConvert.SerializeObject(planingViewModel);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveJobDetail", json);
           // await Clients.Client(Context.ConnectionId).SendAsync("ServerRequest", true);

        }
        public async Task SendMachineData(string data)
        {
            var machineData = JsonConvert.DeserializeObject<MachineData>(data);
            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                  .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();

            if (facName != null && mcName != null)
            {
                await Clients.Group(facName).SendAsync("ReceiveRealTimeData", data);
                if (mcName != "Viewer")
                {
                    await Clients.Group(facName).SendAsync("ReceiveStatusData", machineData.machineState, mcName);
                }
                
            }

        }

        public async Task SendGetOnlineAllUser()
        {
            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                  .Select(c => c.Value).SingleOrDefault();

            if (facName != null)
            {
                var indexUser = listUserID.Where(x => x.factoryName == facName);
                if (indexUser.Count() > 0)
                {
                    foreach (var element in indexUser)
                    {
                        if (element.role == "Machine")
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserOnline", element.machineName);
                        }
                        else
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserOnline", element.email);
                        }
                    }
                }
            }
        }

        public async Task SendMachineError(string timeError, string errorMsg, string desc)
        {
            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                  .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();

            if (facName != null && mcName != null)
            {
                if(desc!="") await Clients.Group(facName).SendAsync("ReceiveRealTimeErrorData", timeError + " >> " + errorMsg + "(" + desc + ")", mcName);
                else await Clients.Group(facName).SendAsync("ReceiveRealTimeErrorData", timeError + " >> " + errorMsg, mcName);
            }

        }
        public async Task SendMessageToMachine(string msg, string time,string machineName)
        {
            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                  .Select(c => c.Value).SingleOrDefault();

            if (facName != null)
            {
                var indexUser = listUserID.FindIndex(x => x.factoryName == facName && x.machineName == machineName && x.role == "Machine");
                if (indexUser != -1)
                {
                    await Clients.Client(listUserID[indexUser].id).SendAsync("ReceiveMessagFromSupervisor", msg);
                }
                else
                {
                    //Save then Send to User when online
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessageConference", machineName, "I'm offine now !!");
                }
            }

        }
        public async Task SendMessageToSupervisor(string msg, string time, string supEmail)
        {
            var claims = Context.User.Claims;
            var facName = claims.Where(c => c.Type == "FactoryName")
                  .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();

            if (facName != null)
            {
                var indexUser = listUserID.FindIndex(x => x.factoryName == facName && x.email == supEmail && x.role == "User");
                if (indexUser != -1)
                {
                    await Clients.Client(listUserID[indexUser].id).SendAsync("ReceiveMessageConference", mcName, msg);
                }
                else
                {
                    //Save then Send to User when online
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessagFromSupervisor", "I'm offline now !!");
                }
            }

        }
        public async Task TrigerRealTimeMachine(string machineName, string factoryName, bool action,int everyTimes=60)
        {

            var indexUser = listUserID.FindIndex(x => x.factoryName == factoryName && x.machineName == machineName);
            if (indexUser != -1)
            {
                await Clients.Client(listUserID[indexUser].id).SendAsync("ServerRequestRealTime", action, everyTimes);
                
                if(!action) await Clients.Client(Context.ConnectionId).SendAsync("ReceiveStatusData", -2, machineName);
                else await Clients.Client(Context.ConnectionId).SendAsync("ReceiveStatusData", -1, machineName);

            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveStatusData", -2, machineName);
            }
        }

        //// [HubMethodName("SendMessageToUser")]
        //public Task SendPrivateMessage(string user, string message)
        //{
        //    //return Clients.User(Context.ConnectionId).SendAsync("ReceiveMessage", message);
        //    return Clients.Client(listUserID[0].id).SendAsync("ReceiveMessage", user, message);
        //    //return Clients.Caller.SendAsync("ReceiveMessage", message);
        //}
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }

        public ChannelReader<int> Counter(
        int count,
        int delay,
        CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<int> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }

            writer.Complete(localException);
        }
        public async Task UploadStream(ChannelReader<string> stream)
        {
            while (await stream.WaitToReadAsync())
            {
                while (stream.TryRead(out var item))
                {
                    // do something with the stream item
                    Console.WriteLine(item);
                }
            }
        }

    }

}
