namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPlayerBotsToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerBots", "Image", c => c.Binary());
            AddColumn("dbo.PlayerBots", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlayerBots", "Owner_Id");
            AddForeignKey("dbo.PlayerBots", "Owner_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.PlayerBots", "UserId");
            DropColumn("dbo.PlayerBots", "Owner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerBots", "Owner", c => c.String());
            AddColumn("dbo.PlayerBots", "UserId", c => c.String());
            DropForeignKey("dbo.PlayerBots", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PlayerBots", new[] { "Owner_Id" });
            DropColumn("dbo.PlayerBots", "Owner_Id");
            DropColumn("dbo.PlayerBots", "Image");
        }
    }
}
