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
        Boolean phase2 = false;
        Vector2 shootPos;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void SetDefaults() {
            npc.aiStyle = -1;
            npc.lifeMax = 6000;
            npc.damage = 30;
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
			music = MusicID.PirateInvasion;
        }

        public override void AI() {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
            Vector2 direction;
            float rotation;

            npc.netAlways = true;

            if(npc.life <= npc.lifeMax / 2) {
                npc.ai[0] = -1;
                npc.ai[1]++;
            } else {
                npc.ai[0]++;
            }


            if(player.dead || !player.active) {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y -= 0.1f;
                if(npc.timeLeft > 20) { 
                    npc.timeLeft = 20;
                    return;
                }
            }
            
            float distance = Vector2.Distance(target, npc.Center);
            if(npc.ai[0] != -1) {
                if(npc.ai[0] < 100) {
                    npc.damage = 30;
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
                    if(npc.ai[0] % 80 == 0) {
                        npc.spriteDirection = 0;
                        npc.damage = 40;
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
                        Main.PlaySound(SoundID.Roar);
                    } else {
                        frame = 2;
                    }
                    npc.netUpdate = true;
                } else if(npc.ai[0] >= 650 && npc.ai[0] < 1000) {
                    frame = 2;
                    npc.damage = 30;

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
                    if(npc.position.X > player.position.X) {
                        shootPos = new Vector2(npc.Center.X - 100, npc.Center.Y -50);
                    } else {
                        shootPos = new Vector2(npc.Center.X + 100, npc.Center.Y - 50);
                    }

                    float accuracy = 5f * (npc.life / npc.lifeMax);
                    Vector2 shootVel = target - shootPos + new Vector2(Main.rand.NextFloat(-accuracy, accuracy), Main.rand.NextFloat(-accuracy, accuracy));
                    shootVel.Normalize();
                    shootVel *= 7.5f;
                    Projectile.NewProjectile(shootPos.X + (float) (-100 * npc.direction) + (float) Main.rand.Next(-40, 41), shootPos.Y - (float) Main.rand.Next(-50, 40), shootVel.X, shootVel.Y, ProjectileID.FlamesTrap, npc.damage, 5f);
                    npc.netUpdate = true;
                } else if(npc.ai[0] > 1000) {
                    npc.ai[0] = 100;
                }
            } else if(npc.ai[1] != -1) {
                //phase 2
                if(npc.ai[1] < 70) {
                    frame =3;
                    if(!phase2) {
                        Main.PlaySound(SoundID.Roar);
                        phase2 = true;
                    }
                }
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