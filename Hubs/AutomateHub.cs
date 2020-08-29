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
using AutomateBussiness.Models.ConferenceModels;

namespace AutomateBussiness.Hubs
{

    //[Authorize(Roles ="Machine")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutomateHub:Hub
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly AutomateBussinessContext _context;

        public AutomateHub(AutomateBussinessContext context, UserManager<AccountViewModel> userManager,
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
            public string factoryID;
            public string machineID;
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
            var facID = claims.Where(c => c.Type == "FactoryID")
                   .Select(c => c.Value).SingleOrDefault();
            var mcID = claims.Where(c => c.Type == "MachineID")
                   .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();
            var roleType= claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == facID);
            var accountDetail = _context.OrganizationTable
                    .SingleOrDefault(m => m.email == emailAddress && m.factoryID == facID);

            if (factory.Count() > 0 && mcID != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, facID);
                var currentUser = new UserList
                {
                    id = Context.ConnectionId,
                    email = emailAddress,
                    factoryID = facID,
                    machineID = mcID,
                    machineName = mcName,
                    role = roleType
                };
                if(mcID != "Viewer")
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ServerRequestRealTime", true, 60);
                    await Clients.Group(facID).SendAsync("ReceiveUserOnline", mcName);
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserImage", accountDetail.photo);
                    await Clients.Group(facID).SendAsync("ReceiveUserOnline", emailAddress);
                }

                
                listUserID.Add(currentUser);
            }
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                   .Select(c => c.Value).SingleOrDefault();
            var mcID = claims.Where(c => c.Type == "MachineID")
                   .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                    .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == facID);

            if (factory.Count() > 0 && mcName != null)
            {
                if (mcName != "Viewer")
                {
                    await Clients.Group(facID).SendAsync("ReceiveStatusData", -2, mcName);
                    await Clients.Group(facID).SendAsync("ReceiveUserOffline", mcName);
                }
                else
                {
                     await Clients.Group(facID).SendAsync("ReceiveUserOffline", emailAddress);
                }
               
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, facID);
                var indexUser = listUserID.FindIndex(x => x.id == Context.ConnectionId);
                listUserID.RemoveAt(indexUser);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendRTCMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveRTCMessage", message);
        }
        public async Task SendMessage(string user, string message)
        {
            var claims = Context.User.Claims;
            var facID= claims.Where(c => c.Type == "FactoryID")
                   .Select(c => c.Value).SingleOrDefault();
            if(facID != null)
            {
                var movie = await _context.Movie
                    .FirstOrDefaultAsync(m => m.Id == System.Convert.ToInt32(user));

                if (movie != null)
                {
                    string json = JsonConvert.SerializeObject(movie);
                    //await Clients.All.SendAsync("ReceiveData", movie.Price, json);
                    await Clients.Group(facID).SendAsync("ReceiveData", movie.Price.ToString(), json);
                    await Clients.Group(facID).SendAsync("ReceiveMessage", decimal.ToDouble(movie.Price), json);
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
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();

            if (facID != null && mcName != null)
            {
                await Clients.Group(facID).SendAsync("ReceiveRealTimeData", data);
                if (mcName != "Viewer")
                {
                    await Clients.Group(facID).SendAsync("ReceiveStatusData", machineData.machineState, mcName);
                }
                
            }

        }
        public async Task SendSearchMachine(string machinehashid,bool IsHashId)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
           
            if (facID != null)
            {
                if (IsHashId)
                {
                    var machine = await _context.MachineTable
                        .FirstOrDefaultAsync(m => m.machineHashID == machinehashid && m.factoryID == facID);

                    if (machine != null)
                    {
                        string json = JsonConvert.SerializeObject(machine);
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSearchMachine", json);
                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSearchMachine", "");
                    }
                }
                else
                {
                    var machine = await _context.MachineTable
                        .FirstOrDefaultAsync(m => m.name == machinehashid && m.factoryID == facID);

                    if (machine != null)
                    {
                        string json = JsonConvert.SerializeObject(machine);
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSearchMachine", json);
                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSearchMachine", "");
                    }
                }
                
            }
        }
        public async Task SendAddRelationWithMachine(string machinehashid, bool IsHashId)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                    .Select(c => c.Value).SingleOrDefault();
            if (facID != null)
            {
                MachineViewModel machine = null;

                if (IsHashId)
                {
                    machine = await _context.MachineTable
                        .FirstOrDefaultAsync(m => m.machineHashID == machinehashid && m.factoryID == facID);
                }
                else
                {
                    machine = await _context.MachineTable
                        .FirstOrDefaultAsync(m => m.name == machinehashid && m.factoryID == facID);
                }


                if (machine != null)
                {
                    var findIsHasRelation =  _context.RelationshipsTable
                       .Where(m => m.relationType == RelationType.machines
                       && (m.requestId == machine.machineHashID || m.responedId == machine.machineHashID));
                       
                    if (findIsHasRelation.Count()>0)
                    {
                        var findUserRelatedwithMachine = findIsHasRelation.Where(m => m.requestId == emailAddress || m.responedId == emailAddress);
                        if (findUserRelatedwithMachine.Count() > 0)
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRelationResult", "has");
                        }
                        else
                        {
                           var newRelation = new Relationship
                           {
                               id = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                               relationDate = DateTime.Now,
                                requestId= emailAddress,
                                responedId = machine.machineHashID,
                                requestById = emailAddress,
                                relationType =RelationType.machines,
                                relationStatus = RelationStatus.accepted,
                                isFavorites = true
                           };
                            _context.Add(newRelation);
                           await _context.SaveChangesAsync();

                           string json = JsonConvert.SerializeObject(machine);
                           await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRelationResult", json);
                        }
                    }
                    else
                    {
                        var newRelation = new Relationship
                        {
                            id = Guid.NewGuid().ToString()+ Guid.NewGuid().ToString(),
                            relationDate = DateTime.Now,
                            requestId = emailAddress,
                            responedId = machine.machineHashID,
                            requestById = emailAddress,
                            relationType = RelationType.machines,
                            relationStatus = RelationStatus.accepted,
                            isFavorites = true
                        };
                        try
                        {
                            _context.Add(newRelation);
                            await _context.SaveChangesAsync();
                        }
                        catch(Exception ex)
                        {

                        }
                       

                        string json = JsonConvert.SerializeObject(machine);
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRelationResult", json);
                    }

                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRelationResult", "error");
                }

            }
        }
        public async Task SendGetOnlineAllUser()
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();


            if (facID != null)
            {
                var indexUser = listUserID.Where(x => x.factoryID == facID);
                if (indexUser.Count() > 0)
                {
                    foreach (var element in indexUser)
                    {
                        if (element.role == "Machine")
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserOnline", element.machineName);

                            //var machineName = await _context.MachineTable.FirstOrDefaultAsync(c => c.machineHashID == element.machineID
                            //                        && c.factoryID == facID);

                            //if (machineName != null)
                            //{
                                
                            //}  
                        }
                        else
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUserOnline", element.email);
                        }
                    }
                }
            }
        }
        public async Task SendGetMachineContactsList()
        {
            var claims = Context.User.Claims;
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();

            // find chat Relationships
            IEnumerable<Relationship> _relationships = _context.RelationshipsTable.Where(r => r.requestId == emailAddress || r.responedId == emailAddress);

            //get all my machines
            var genreMachineId = from r in _relationships
                                 where r.relationType == RelationType.machines
                                 select r.responedId;

            // find all my contacts machines
            IEnumerable<MachineViewModel> _machines = null;
            if (genreMachineId.Count() > 0)
            {
                _machines = _context.MachineTable.Where(g => genreMachineId.Contains(g.machineHashID)).ToList();
            }
            string json = JsonConvert.SerializeObject(_machines);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAddMachineToContactsList", json);

        }
        public async Task SendMachineError(string timeError, string errorMsg, string desc)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var mcName = claims.Where(c => c.Type == "MachineName")
                   .Select(c => c.Value).SingleOrDefault();

            if (facID != null && mcName != null)
            {
                if(desc!="") await Clients.Group(facID).SendAsync("ReceiveRealTimeErrorData", timeError + " >> " + errorMsg + "(" + desc + ")", mcName);
                else await Clients.Group(facID).SendAsync("ReceiveRealTimeErrorData", timeError + " >> " + errorMsg, mcName);
            }

        }
        public async Task SendMessageToMachine(string msg,string machineName)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();
            var machine = await _context.MachineTable
                  .FirstOrDefaultAsync(m => m.name == machineName && m.factoryID == facID);

            if (facID != null)
            {
                var newChat = new ChatHistorys
                {
                    id = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                    messageDate = DateTime.Now,
                    senderId = emailAddress,
                    receiverId = machine.machineHashID,
                    message = msg,
                    messageType = MessageType.text,
                    senderMessageStatus = MessageStatus.read,
                    receiverMessageStatus = MessageStatus.unread,
                };

                var indexUser = listUserID.FindIndex(x => x.factoryID == facID && x.machineName == machineName && x.role == "Machine");

                if (indexUser != -1)
                {
                    newChat.receiverMessageStatus = MessageStatus.read;
                    await Clients.Client(listUserID[indexUser].id).SendAsync("ReceiveMessagFromSupervisor", msg);
                }
                else
                {
                    //Save then Send to User when online
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessageConference", machineName, "I'm offine now !!", machine.machineImage);
                }

                _context.Add(newChat);
                await _context.SaveChangesAsync();
            }

        }
        public async Task SendClearMessageByUser(string machineName)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();
            var machine = await _context.MachineTable
                  .FirstOrDefaultAsync(m => m.name == machineName && m.factoryID == facID);

            if (facID != null && machine != null)
            {
                var chatHistory = _context.ChatHistorysTable
                    .Where(m => (((m.senderId == emailAddress && m.receiverId == machine.machineHashID && m.senderMessageStatus != MessageStatus.delete)
                    || (m.senderId == machine.machineHashID && m.receiverId == emailAddress && m.receiverMessageStatus != MessageStatus.delete))));

                if (chatHistory.Count() > 0)
                {
                    foreach(ChatHistorys lsChat in chatHistory)
                    {
                        if(lsChat.senderId == emailAddress)
                        {
                            lsChat.senderMessageStatus = MessageStatus.delete;
                        }
                        if(lsChat.receiverId == emailAddress)
                        {
                            lsChat.receiverMessageStatus = MessageStatus.delete;
                        }
                        
                    }
                    await _context.SaveChangesAsync();
                }
            }

        }
        public async Task SendLoadMessageByUser(string machineName)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var emailAddress = claims.Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();
            var machine = await _context.MachineTable
                  .FirstOrDefaultAsync(m => m.name == machineName && m.factoryID == facID);

            if (facID != null && machine != null )
            {
                var chatHistory = _context.ChatHistorysTable
                   .Where(m => (((m.senderId == emailAddress && m.receiverId == machine.machineHashID && m.senderMessageStatus != MessageStatus.delete)
                    || (m.senderId == machine.machineHashID && m.receiverId == emailAddress && m.receiverMessageStatus != MessageStatus.delete))))
                   .OrderBy(m=> m.messageDate);

                if (chatHistory.Count() > 0)
                {
                    //Send last chat history
                    string json = JsonConvert.SerializeObject(chatHistory);
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveLoadMessageToUser", json, emailAddress);
                }
            }

        }

        // machine function
        public async Task SendClearMessageByMachine(string supEmail)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var mcID = claims.Where(c => c.Type == "MachineID")
                  .Select(c => c.Value).SingleOrDefault();

            if (facID != null && mcID != null)
            {
                var chatHistory = _context.ChatHistorysTable
                    .Where(m => (((m.senderId == supEmail && m.receiverId == mcID && m.receiverMessageStatus != MessageStatus.delete)
                    || (m.senderId == mcID && m.receiverId == supEmail && m.senderMessageStatus != MessageStatus.delete))));

                if (chatHistory.Count() > 0)
                {
                    foreach (ChatHistorys lsChat in chatHistory)
                    {
                        if (lsChat.senderId == mcID)
                        {
                            lsChat.senderMessageStatus = MessageStatus.delete;
                        }
                        if (lsChat.receiverId == mcID)
                        {
                            lsChat.receiverMessageStatus = MessageStatus.delete;
                        }

                    }
                    await _context.SaveChangesAsync();
                }
            }

        }
        public async Task SendLoadMessageByMachine(string supEmail)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var mcID = claims.Where(c => c.Type == "MachineID")
                  .Select(c => c.Value).SingleOrDefault();

            if (facID != null && mcID != null)
            {
                var chatHistory = _context.ChatHistorysTable
                   .Where(m => (((m.senderId == supEmail && m.receiverId == mcID && m.receiverMessageStatus != MessageStatus.delete)
                    || (m.senderId == mcID && m.receiverId == supEmail && m.senderMessageStatus != MessageStatus.delete))))
                   .OrderBy(m => m.messageDate);

                if (chatHistory.Count() > 0)
                {
                    //Send last chat history
                    string json = JsonConvert.SerializeObject(chatHistory);
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveLoadMessageToMachine", json);
                }
            }

        }
        public async Task SendMessageToSupervisor(string msg, string supEmail)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();
            var mcID = claims.Where(c => c.Type == "MachineID")
                  .Select(c => c.Value).SingleOrDefault();
            var machine = await _context.MachineTable
                  .FirstOrDefaultAsync(m => m.machineHashID == mcID && m.factoryID == facID);

            if (facID != null)
            {
                var newChat = new ChatHistorys
                {
                    id = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                    messageDate = DateTime.Now,
                    senderId = mcID,
                    receiverId = supEmail,
                    message = msg,
                    messageType = MessageType.text,
                    senderMessageStatus = MessageStatus.read,
                    receiverMessageStatus = MessageStatus.unread,
                };

                var indexUser = listUserID.FindIndex(x => x.factoryID == facID && x.email == supEmail && x.role == "User");
                if (indexUser != -1)
                {
                    newChat.receiverMessageStatus = MessageStatus.read;
                    await Clients.Client(listUserID[indexUser].id).SendAsync("ReceiveMessageConference", machine.name, msg,machine.machineImage);
                }
                else
                {
                    //Save then Send to User when online
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessagFromSupervisor", "I'm offline now !!");
                }
                _context.Add(newChat);
                await _context.SaveChangesAsync();
            }

        }
        public async Task TrigerRealTimeMachine(string machineName, bool action,int everyTimes=60)
        {
            var claims = Context.User.Claims;
            var facID = claims.Where(c => c.Type == "FactoryID")
                  .Select(c => c.Value).SingleOrDefault();

            var indexUser = listUserID.FindIndex(x => x.factoryID == facID && x.machineName == machineName);
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
