namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedGameHistoryClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameSummaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Player1FinalBotCount = c.Int(nullable: false),
                        Player2FinalBotCount = c.Int(nullable: false),
                        Player1FinalEnergy = c.Int(nullable: false),
                        Player2FinalEnergy = c.Int(nullable: false),
                        TournamentGame = c.Boolean(nullable: false),
                        Player1_Id = c.Int(),
                        Player2_Id = c.Int(),
                        Winner_Id = c.Int(),
                        PlayerBot_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayerBots", t => t.Player1_Id)
                .ForeignKey("dbo.PlayerBots", t => t.Player2_Id)
                .ForeignKey("dbo.PlayerBots", t => t.Winner_Id)
                .ForeignKey("dbo.PlayerBots", t => t.PlayerBot_Id)
                .Index(t => t.Player1_Id)
                .Index(t => t.Player2_Id)
                .Index(t => t.Winner_Id)
                .Index(t => t.PlayerBot_Id);
            
            DropColumn("dbo.PlayerBots", "Wins");
            DropColumn("dbo.PlayerBots", "Losses");
            DropColumn("dbo.PlayerBots", "Ties");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerBots", "Ties", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerBots", "Losses", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerBots", "Wins", c => c.Int(nullable: false));
            DropForeignKey("dbo.GameSummaries", "PlayerBot_Id", "dbo.PlayerBots");
            DropForeignKey("dbo.GameSummaries", "Winner_Id", "dbo.PlayerBots");
            DropForeignKey("dbo.GameSummaries", "Player2_Id", "dbo.PlayerBots");
            DropForeignKey("dbo.GameSummaries", "Player1_Id", "dbo.PlayerBots");
            DropIndex("dbo.GameSummaries", new[] { "PlayerBot_Id" });
            DropIndex("dbo.GameSummaries", new[] { "Winner_Id" });
            DropIndex("dbo.GameSummaries", new[] { "Player2_Id" });
            DropIndex("dbo.GameSummaries", new[] { "Player1_Id" });
            DropTable("dbo.GameSummaries");
        }
    }
}
