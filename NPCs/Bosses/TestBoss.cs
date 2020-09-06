using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace iridium.NPCs.Bosses {
    class TestBoss : ModNPC {

        //AI
        Boolean stunned;
        int stunnedTimer;
        int attackTimer = 0;
        Boolean fastSpeed = false;

        int frame = 0;
        //double counting;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults() {
            npc.aiStyle = -1;
            npc.lifeMax = 4000;
            npc.damage = 100;
            npc.defense = 50;
            npc.knockBackResist = 0f;
            npc.width = 100;
            npc.height = 100;
            npc.value = Item.buyPrice(0, 20, 0, 0);
            npc.boss = true;
            npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
			music = MusicID.Boss2;
        }

        public override void AI() {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
            Vector2 direction;
            float rotation;
            npc.netAlways = true;
            npc.TargetClosest(true);
            
            if(player.dead || !player.active || Main.dayTime) {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y -= 0.1f;
                if(npc.timeLeft > 20) { 
                    npc.timeLeft = 20;
                    return;
                }
            }

            if(stunned) {
                npc.velocity.X = 0.0f;
                npc.velocity.Y = 0.0f;

                stunnedTimer++;

                if(stunnedTimer >= 100) {
                    stunned = false;
                    stunnedTimer = 0;
                }
            }

            npc.ai[0]++;
            float distance = Vector2.Distance(target, npc.Center);
            if(npc.ai[0] < 300) {
                frame = 0;
                direction = npc.Center - player.Center;
                rotation = (float) Math.Atan2(direction.Y, direction.X);
                npc.rotation = rotation + ((float)Math.PI * 0.5f);
                npc.damage = 100;
                MoveTwards(npc, target, (distance > 300 ? 13f : 7f), 30f);
                npc.netUpdate = true;
            } else if(npc.ai[0] >= 300 && npc.ai[0] < 450) {
                direction = npc.Center - player.Center;
                rotation = (float) Math.Atan2(direction.Y, direction.X);
                npc.rotation = rotation + ((float)Math.PI * 0.5f);
                stunned = true;
                npc.damage = 50;
                npc.defense = 100;
                MoveTwards(npc, target, (distance > 300 ? 13f : 7f), 30f);
                npc.netUpdate = true;
            } else if(npc.ai[0] > 450) {
                stunned = false;
                npc.damage = 200;
                npc.defense = 50;

                if(!fastSpeed) {
                    direction = npc.Center - player.Center;
                    rotation = (float) Math.Atan2(direction.Y, direction.X);
                    npc.rotation = rotation + ((float)Math.PI * 0.5f);
                    fastSpeed = true;
                } else {
                    if(npc.ai[0] % 50 == 0) {
                        frame = 1;
                        float speed = 14f;
                        Vector2 vector = new Vector2(npc.position.X + (float) npc.width * 0.5f, npc.position.Y + (float) npc.height * 0.5f);
                        float x = player.position.X + (float) (player.width / 2) - vector.X;
                        float y = player.position.Y + (float) (player.height / 2) - vector.Y;
                        float distance2 = (float)Math.Sqrt(x * x + y * y);
                        float factor = speed / distance2;
                        direction = npc.Center - player.Center;
                        rotation = (float) Math.Atan2(direction.Y, direction.X);
                        npc.rotation = rotation + ((float)Math.PI * 0.5f);
                        npc.velocity.X = x * factor;
                        npc.velocity.Y = y * factor;
                    }
                }
                npc.netUpdate = true;
            }

            if(npc.ai[0] > 650) {
                npc.ai[0] = 0;
                fastSpeed = false;
            }
        }

        public override void FindFrame(int FrameHeight) {
            if(frame == 0) {
                npc.frame.Y = 0;
            }

            if(frame == 1) {
                npc.frame.Y = FrameHeight;
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