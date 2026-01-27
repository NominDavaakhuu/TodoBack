namespace TodoBack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialR : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TodoItems", "UserId", "dbo.Users");
            AddColumn("dbo.TodoItems", "DeletedAt", c => c.DateTime());
            CreateIndex("dbo.Users", "UsernameNormalized", unique: true, name: "IX_User_UsernameNormalized");
            CreateIndex("dbo.Users", "EmailNormalized", unique: true, name: "IX_User_EmailNormalized");
            AddForeignKey("dbo.TodoItems", "UserId", "dbo.Users", "Id");
            DropColumn("dbo.TodoItems", "DeleteAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TodoItems", "DeleteAt", c => c.DateTime());
            DropForeignKey("dbo.TodoItems", "UserId", "dbo.Users");
            DropIndex("dbo.Users", "IX_User_EmailNormalized");
            DropIndex("dbo.Users", "IX_User_UsernameNormalized");
            DropColumn("dbo.TodoItems", "DeletedAt");
            AddForeignKey("dbo.TodoItems", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
