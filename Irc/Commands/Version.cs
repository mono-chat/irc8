﻿using Irc.Resources;

namespace Irc.Commands;

public class Version : Command
{
    public Version() : base(0, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raws.IRCX_RPL_VERSION_351(chatFrame.Server, chatFrame.User, chatFrame.Server.ServerVersion));
    }
}