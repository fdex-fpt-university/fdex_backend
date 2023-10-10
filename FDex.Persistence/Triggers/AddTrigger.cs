using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FDex.Persistence.Triggers
{
    public partial class AddTrigger : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE TRIGGER [UserLevelTrigger]
            ON [Users]
            AFTER INSERT, UPDATE
            AS
            BEGIN
                IF (SELECT [TradePoint] FROM INSERTED) >= 1000 
                   AND (SELECT [ReferralPoint] FROM INSERTED) >= 1000
                BEGIN
                    UPDATE [Users]
                    SET [Level] = [Level] + 1
                    WHERE [Wallet] = (SELECT [Wallet] FROM INSERTED)
                END
            END
        ");
        }
    }
}

