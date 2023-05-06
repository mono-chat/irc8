﻿using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;

namespace Irc.Commands;

public class Join : Command, ICommand
{
    public Join() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var channels = chatFrame.Message.Parameters.First();
        var key = chatFrame.Message.Parameters.Count > 1 ? chatFrame.Message.Parameters[1] : string.Empty;

        var channelNames = ValidateChannels(server, user, channels);
        if (channelNames.Count == 0) return;

        JoinChannels(server, user, channelNames, key);
    }

    public static List<string> ValidateChannels(IServer server, IUser user, string channels)
    {
        var channelNames = channels.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (channelNames.Count == 0)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, string.Empty));
        }
        else
        {
            var invalidChannelNames = channelNames.Where(c => !Channel.ValidName(c)).ToList();
            channelNames.RemoveAll(c => invalidChannelNames.Contains(c));

            // TODO: Could do better below for reporting invalid channel / empty channel
            if (invalidChannelNames.Count > 0)
                invalidChannelNames.ForEach(c => user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, c)));
        }

        return channelNames;
    }

    public static void JoinChannels(IServer server, IUser user, List<string> channelNames, string key)
    {
        // TODO: Optimize the below code
        foreach (var channelName in channelNames)
        {
            var channel = server
                            .GetChannels()
                                .FirstOrDefault(c => c.GetName().ToUpper() == channelName.ToUpper());



            if (channel == null) channel = Create(server, user, channelName, key);

            EnumChannelAccessResult channelAccessResult = channel.GetAccess(user, key, false);

            if (!channel.Allows(user) || channelAccessResult < EnumChannelAccessResult.SUCCESS_GUEST) {
                SendJoinError(server, channel, user, channelAccessResult);
                continue;
            }

            channel.Join(user, channelAccessResult)
            .SendTopic(user)
            .SendNames(user);
        }
    }

    public static void SendJoinError(IServer server, IChannel channel, IUser user, EnumChannelAccessResult result)
    {
        switch (result) {
            case EnumChannelAccessResult.ERR_BADCHANNELKEY:
                {
                    user.Send(Raw.IRCX_ERR_BADCHANNELKEY_475(server, user, channel));
                    break;
                }
            case EnumChannelAccessResult.ERR_INVITEONLYCHAN:
                {
                    user.Send(Raw.IRCX_ERR_INVITEONLYCHAN_473(server, user, channel));
                    break;
                }
            case EnumChannelAccessResult.ERR_CHANNELISFULL:
                {
                    user.Send(Raw.IRCX_ERR_CHANNELISFULL_471(server, user, channel));
                    break;
                }
            default:{
                user.Send($"CANNOT JOIN CHANNEL {result.ToString()}");
                break;
            }
        }
    }

    public static IChannel Create(IServer server, IUser user, string name, string key)
    {
        var channel = server.CreateChannel(name);
        channel.ChannelStore.Set("topic", name);
        channel.ChannelStore.Set("key", key);
        channel.Modes.NoExtern = true;
        channel.Modes.TopicOp = true;
        channel.Modes.UserLimit = 50;
        server.AddChannel(channel);
        return channel;
    }
}