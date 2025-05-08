using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.CrewmateRoles.ClericMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Pyromaniac : Role
    {
        private KillButton _igniteButton;
        public bool PyromaniacWins;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public bool LastKiller = false;

        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);


        public Pyromaniac(PlayerControl player) : base(player)
        {
            Name = "Pyromaniac";
            ImpostorText = () => "Douse Players And Ignite The Light";
            TaskText = () => "Douse players and ignite to kill all douses\nFake Tasks:";
            Color = Patches.Colors.Arsonist;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Pyromaniac;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.IsLover()) == 2) return false;
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.IsCrewKiller())) > 1) return false;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Crewmates))) == 1 ||
                PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2)
            {
                Utils.Rpc(CustomRPC.PyromaniacWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }


        public void Wins()
        {
            PyromaniacWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var pyromaniacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            pyromaniacTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = pyromaniacTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.PyroDouseCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && !player.IsBarriered() && player != ShowShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    foreach (var medic in player.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, player.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }
                else if (player.IsBarriered())
                {
                    foreach (var cleric in player.GetCleric())
                    {
                        StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                    }
                }
            }
            DousedPlayers.Clear();
        }
    }
}
