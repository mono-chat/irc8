﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class UserLimit : ModeRuleChannel
{
    public UserLimit() : base(Tokens.ChannelModeUserLimit, true)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var result = base.Evaluate(source, target, flag, parameter);
        if (result != EnumIrcError.OK) return result;

        var user = (Objects.User)source;
        var channel = (Objects.Channel)target;
        var isAdministrator = user.IsAdministrator();

        if (flag == false)
        {
            if (isAdministrator)
            {
                // TODO: Currently does not support unsetting limit without extra parameter

                channel.UserLimit = 0;
                DispatchModeChange(source, target, false, string.Empty);
            }

            return EnumIrcError.OK;
        }


        if (!int.TryParse(parameter, out var limit)) return EnumIrcError.ERR_NEEDMOREPARAMS;

        if (limit > 0 && (limit <= 100 || isAdministrator))
        {
            channel.UserLimit = limit;
            DispatchModeChange(source, target, true, limit.ToString());
        }

        return EnumIrcError.OK;
    }
}