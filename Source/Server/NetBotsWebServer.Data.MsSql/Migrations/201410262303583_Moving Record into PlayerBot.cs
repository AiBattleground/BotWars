namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovingRecordintoPlayerBot : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerBots", "Record_Id", "dbo.BotRecords");
            DropIndex("dbo.PlayerBots", new[] { "Record_Id" });
            AddColumn("dbo.PlayerBots", "Wins", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerBots", "Losses", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerBots", "Ties", c => c.Int(nullable: false));
            DropColumn("dbo.PlayerBots", "Record_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerBots", "Record_Id", c => c.Int());
            DropColumn("dbo.PlayerBots", "Ties");
            DropColumn("dbo.PlayerBots", "Losses");
            DropColumn("dbo.PlayerBots", "Wins");
            CreateIndex("dbo.PlayerBots", "Record_Id");
            AddForeignKey("dbo.PlayerBots", "Record_Id", "dbo.BotRecords", "Id");
        }
    }
}
