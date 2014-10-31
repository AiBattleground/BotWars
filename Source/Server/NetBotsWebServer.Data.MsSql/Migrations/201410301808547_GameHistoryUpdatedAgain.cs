namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameHistoryUpdatedAgain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerBots", "GameSummary_Id", "dbo.GameSummaries");
            DropIndex("dbo.PlayerBots", new[] { "GameSummary_Id" });
            AddColumn("dbo.GameSummaries", "Player1_Id", c => c.Int());
            AddColumn("dbo.GameSummaries", "Player2_Id", c => c.Int());
            CreateIndex("dbo.GameSummaries", "Player1_Id");
            CreateIndex("dbo.GameSummaries", "Player2_Id");
            AddForeignKey("dbo.GameSummaries", "Player1_Id", "dbo.PlayerBots", "Id");
            AddForeignKey("dbo.GameSummaries", "Player2_Id", "dbo.PlayerBots", "Id");
            DropColumn("dbo.PlayerBots", "GameSummary_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerBots", "GameSummary_Id", c => c.Int());
            DropForeignKey("dbo.GameSummaries", "Player2_Id", "dbo.PlayerBots");
            DropForeignKey("dbo.GameSummaries", "Player1_Id", "dbo.PlayerBots");
            DropIndex("dbo.GameSummaries", new[] { "Player2_Id" });
            DropIndex("dbo.GameSummaries", new[] { "Player1_Id" });
            DropColumn("dbo.GameSummaries", "Player2_Id");
            DropColumn("dbo.GameSummaries", "Player1_Id");
            CreateIndex("dbo.PlayerBots", "GameSummary_Id");
            AddForeignKey("dbo.PlayerBots", "GameSummary_Id", "dbo.GameSummaries", "Id");
        }
    }
}
