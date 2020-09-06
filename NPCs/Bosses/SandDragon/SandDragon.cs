using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace iridium.NPCs.Bosses.SandDragon {
    class SandDragon : ModNPC {

        //AI
        int frame;
        int waitTime;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void SetDefaults() {
            npc.aiStyle = -1;
            npc.lifeMax = 3000;
            npc.damage = 40;
            npc.defense = 20;
            npc.knockBackResist = 0f;
            npc.width = 250;
            npc.height = 200;
            npc.value = Item.buyPrice(0, 20, 0, 0);
            npc.boss = true;
            npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
			music = MusicID.Boss5;
        }

        public override void AI() {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
            Vector2 direction;
            float rotation;

            npc.netAlways = true;

            if(player.dead || !player.active) {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y -= 0.1f;
                if(npc.timeLeft > 20) { 
                    npc.timeLeft = 20;
                    return;
                }
            }
            
            npc.ai[0]++;
            float distance = Vector2.Distance(target, npc.Center);
            if(npc.ai[0] < 100) {
                npc.damage = 40;
                frame = 0;
                npc.velocity.Y = -0.1f;
                if(npc.position.X > player.position.X) {
                        npc.spriteDirection = 1;
                        direction = player.Center - npc.Center;
                        rotation = (float) Math.Atan2(direction.Y, direction.X) - (float) Math.PI;
                    } else {
                        npc.spriteDirection = 0;
                        direction = player.Center - npc.Center;
                        rotation = (float) Math.Atan2(direction.Y, direction.X);
                    }
                    npc.rotation = rotation;
                npc.netUpdate = true;
            } else if(npc.ai[0] >= 100 && npc.ai[0] < 650) {
                if(npc.ai[0] % 50 == 0) {
                    npc.spriteDirection = 0;
                    npc.damage = 80;
                    frame = 1;
                    float speed = 10f;
                    Vector2 vector = new Vector2(npc.position.X + (float) npc.width * 0.5f, npc.position.Y + (float) npc.height * 0.5f);
                    float x = player.position.X + (float) (player.width / 2) - vector.X;
                    float y = player.position.Y + (float) (player.height / 2) - vector.Y;
                    float distance2 = (float)Math.Sqrt(x * x + y * y);
                    float factor = speed / distance2;
                    if(npc.position.X > player.position.X) {
                        npc.spriteDirection = 1;
                        direction = player.Center - npc.Center;
                        rotation = (float) Math.Atan2(direction.Y, direction.X) - (float) Math.PI;
                    } else {
                        npc.spriteDirection = 0;
                        direction = player.Center - npc.Center;
                        rotation = (float) Math.Atan2(direction.Y, direction.X);
                    }
                    npc.rotation = rotation;
                    npc.velocity.X = x * factor;
                    npc.velocity.Y = y * factor;
                } else {
                    frame = 2;
                }
                npc.netUpdate = true;
            } else if(npc.ai[0] >= 650 && npc.ai[0] < 1000) {
                frame = 2;
                npc.damage = 40;

                if(npc.position.X > player.position.X) {
                    npc.spriteDirection = 1;
                    direction = player.Center - npc.Center;
                    rotation = (float) Math.Atan2(direction.Y, direction.X) - (float) Math.PI;
                } else {
                    npc.spriteDirection = 0;
                    direction = player.Center - npc.Center;
                    rotation = (float) Math.Atan2(direction.Y, direction.X);
                }
                npc.rotation = rotation;
                MoveTwards(npc, target, 5f, 0f);
                npc.netUpdate = true;
            } else if(npc.ai[0] > 1000) {
                npc.ai[0] = 100;
            }
        }

        private void MoveTwards(NPC nPC, Vector2 playerTarget, float speed, float turnResistance) {
            var move = playerTarget - npc.Center;
            float length = move.Length();
            if(length > speed) {
                move *= speed / length;
            }
            
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if(length > speed) {
                move *= speed / length;
            }
            npc.velocity = move;
        }
    }
}