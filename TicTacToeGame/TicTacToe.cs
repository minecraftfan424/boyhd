﻿using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeDiscordBot.TicTacToeGame
{
    public class TicTacToe : BaseCommandModule
    {
        [Command("tictactoe")]
        [Aliases("ttt", "krydsogbolle", "kob", "debugttt")]
        public async Task TicTacToeGame(CommandContext ctx, string userMention, string difficulty = "")
        {
            DiscordMember playerTwo = null;

            List<DiscordMember> dmList = await GetMemberList(ctx);

            await Task.WhenAny(GetMemberList(ctx));
            GameEmoji.InitEmoji(ctx);

            foreach (DiscordMember discordMember in dmList)
            {
                if (discordMember.Mention.Contains(userMention) && discordMember.IsBot)
                {
                    playerTwo = discordMember;

                    if (string.IsNullOrEmpty(difficulty))
                        await ctx.Channel.SendMessageAsync(
                            "Trying to play against the AI? Make sure to include the difficulty.\nFormat: \"!ttt @DiscordBot difficulty\"\nThere are currently 3 difficulties: Easy, Medium & Hard");
                    else
                    {
                        difficulty.ToLower();
                        SingleplayerGame sp = new SingleplayerGame(ctx, playerTwo, difficulty);
                        await sp.BeginSingleplayerGame();
                    }
                }

                else if (discordMember.Mention == userMention)
                {
                    playerTwo = discordMember;
                    MultiplayerGame mp = new MultiplayerGame(ctx, playerTwo);
                    await mp.BeginMultiplayerGame();
                }
            }

            if (playerTwo == null)
                await ctx.Channel.SendMessageAsync("The requested player could not be found. Please try again with another user.");
        }


        public async Task<List<DiscordMember>> GetMemberList(CommandContext ctx)
        {
            List<DiscordMember> memberList = await Task.Run(() => ctx.Channel.Users.ToList());

            return new List<DiscordMember>(memberList);
        }
    }
}
