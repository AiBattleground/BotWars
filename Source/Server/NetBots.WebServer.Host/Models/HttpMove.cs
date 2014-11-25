using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using NetBots.GameEngine;

namespace NetBotsHostProject.Models
{
    public class HttpMove
    {
        public PlayerMoves PlayerMoves { get; set; }
        public HttpMoveResponse HttpMoveResponse { get; set; }
        public HttpRequestException Exception { get; set; }
    }

    public enum HttpMoveResponse
    {
        OK,
        Timeout,
        Error
    }
}