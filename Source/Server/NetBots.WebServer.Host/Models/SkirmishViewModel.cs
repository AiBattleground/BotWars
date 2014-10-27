using NetBots.WebServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetBots.WebServer.Host.Models
{
    public class SkirmishViewModel
    {
        private IList<PlayerBot> _bots;

        public int SelectedBot1Id { get; set; }
        public int SelectedBot2Id { get; set; }

        public SkirmishViewModel(IList<PlayerBot> bots)
        {
            _bots = bots;
        }

        public IEnumerable<SelectListItem> Bots
        {
            get
            {
                return new SelectList(_bots, "Id", "Name");
            }
        }
    }
}