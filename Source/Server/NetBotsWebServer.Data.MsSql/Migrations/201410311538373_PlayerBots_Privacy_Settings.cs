namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerBots_Privacy_Settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerBots", "Private", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerBots", "Private");
        }
    }
}
