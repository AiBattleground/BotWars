namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rankings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerBots", "LadderRank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerBots", "LadderRank");
        }
    }
}
