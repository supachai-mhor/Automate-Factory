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
        
        #region---Data Members---
        static List<string> listUserID = new List<string>();
        #endregion

        public override async Task OnConnectedAsync()
        {
            // var facName = userManager.Users.Where(m => m.UserName == Context.UserIdentifier).First().FactoryName;

            var claims = Context.User.Claims;
            var name = claims.Where(c => c.Type == "FactoryName")
                   .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == name);
            if (factory.Count() > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, name);
                listUserID.Add(Context.ConnectionId);
            }
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var claims = Context.User.Claims;
            var name = claims.Where(c => c.Type == "FactoryName")
                   .Select(c => c.Value).SingleOrDefault();

            var factory = _context.FactoryTable.Where(m => m.factoryName == name);
            if (factory.Count() > 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, name);
                listUserID.RemoveAt(listUserID.IndexOf(Context.ConnectionId));
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
                    await Clients.Group(groupName).SendAsync("ReceiveData", movie.Price, json);
                }
            }
            
            //else
            //{
            //    await Clients.All.SendAsync("ReceiveMessage", user, message);
            //}
            //await Clients.All.SendAsync("ReceiveData", user, message);

        }
       // [HubMethodName("SendMessageToUser")]
        public Task SendPrivateMessage(string user, string message)
        {
            //return Clients.User(Context.ConnectionId).SendAsync("ReceiveMessage", message);
            return Clients.Client(listUserID[0]).SendAsync("ReceiveMessage", user, message);
            //return Clients.Caller.SendAsync("ReceiveMessage", message);
        }
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
