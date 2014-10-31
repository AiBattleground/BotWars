namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameHistoryCreated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameSummaries", "Player1_Id", "dbo.PlayerBots");
            DropForeignKey("dbo.GameSummaries", "Player2_Id", "dbo.PlayerBots");
            DropIndex("dbo.GameSummaries", new[] { "Player1_Id" });
            DropIndex("dbo.GameSummaries", new[] { "Player2_Id" });
            AddColumn("dbo.PlayerBots", "GameSummary_Id", c => c.Int());
            CreateIndex("dbo.PlayerBots", "GameSummary_Id");
            AddForeignKey("dbo.PlayerBots", "GameSummary_Id", "dbo.GameSummaries", "Id");
            DropColumn("dbo.GameSummaries", "Player1FinalBotCount");
            DropColumn("dbo.GameSummaries", "Player2FinalBotCount");
            DropColumn("dbo.GameSummaries", "Player1FinalEnergy");
            DropColumn("dbo.GameSummaries", "Player2FinalEnergy");
            DropColumn("dbo.GameSummaries", "Player1_Id");
            DropColumn("dbo.GameSummaries", "Player2_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameSummaries", "Player2_Id", c => c.Int());
            AddColumn("dbo.GameSummaries", "Player1_Id", c => c.Int());
            AddColumn("dbo.GameSummaries", "Player2FinalEnergy", c => c.Int(nullable: false));
            AddColumn("dbo.GameSummaries", "Player1FinalEnergy", c => c.Int(nullable: false));
            AddColumn("dbo.GameSummaries", "Player2FinalBotCount", c => c.Int(nullable: false));
            AddColumn("dbo.GameSummaries", "Player1FinalBotCount", c => c.Int(nullable: false));
            DropForeignKey("dbo.PlayerBots", "GameSummary_Id", "dbo.GameSummaries");
            DropIndex("dbo.PlayerBots", new[] { "GameSummary_Id" });
            DropColumn("dbo.PlayerBots", "GameSummary_Id");
            CreateIndex("dbo.GameSummaries", "Player2_Id");
            CreateIndex("dbo.GameSummaries", "Player1_Id");
            AddForeignKey("dbo.GameSummaries", "Player2_Id", "dbo.PlayerBots", "Id");
            AddForeignKey("dbo.GameSummaries", "Player1_Id", "dbo.PlayerBots", "Id");
        }
    }
}
