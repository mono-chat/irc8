﻿namespace Irc.Enumerations;

public enum EnumIrcError
{
    OK = -1,
    NONE = 0,
    ERR_ALREADYINCHANNEL = 1,
    ERR_NOSUCHNICK = 401,
    ERR_NOSUCHCHANNEL = 403,
    ERR_NICKINUSE = 433,
    ERR_NOTONCHANNEL = 442,
    ERR_NEEDMOREPARAMS = 461,
    ERR_KEYSET = 467,
    ERR_CHANNELISFULL = 471,
    ERR_UNKNOWNMODE = 472,
    ERR_INVITEONLYCHAN = 473,
    ERR_BANNEDFROMCHAN = 474,
    ERR_BADCHANNELKEY = 475,
    ERR_NOIRCOP = 481,
    ERR_NOCHANOP = 482,
    ERR_NOCHANOWNER = 485,
    ERR_UNKNOWNMODEFLAG = 501,
    ERR_CANNOTSETFOROTHER = 502,
    ERR_SECUREONLYCHAN = 557,
    ERR_AUTHONLYCHAN = 904,
    ERR_BADVALUE = 906,
    ERR_NOPERMS = 908
}