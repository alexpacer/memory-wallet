using System;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.Lib.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IFileDirHelper _fileDirHelper;
        private readonly string _playerRepoFolder;

        public PlayerRepository(IFileDirHelper fileDirHelper)
        {
            _fileDirHelper = fileDirHelper;
            _playerRepoFolder = _fileDirHelper.BuildPath(
                EnvVar.DataStorageLocation,
                "players");
            _fileDirHelper.CreateIfNotExists(_playerRepoFolder);
        }

        PlayerProfile IPlayerRepository.GetPlayer(long id)
        {
            var playerData = _fileDirHelper.BuildPath(_playerRepoFolder, id.ToString());
            if (!_fileDirHelper.FileExists(playerData))
                throw new Exception($"Player Data does not exists {playerData}");

            var ms = _fileDirHelper.ReadAllText(playerData);
            var playerProfile = Extensions.ConvertToPlayerProfile(ms);

            return playerProfile;
        }

        void IPlayerRepository.CreatePlayer(PlayerProfile profile)
        {
            var playerData = _fileDirHelper.BuildPath(
                _playerRepoFolder,
                profile.Id.ToString());

            if (!_fileDirHelper.FileExists(playerData))
            {
                var playerStream = profile.ToJsonString();
                _fileDirHelper.CreateTextFile(playerData, playerStream);
            }
        }
    }

    public interface IPlayerRepository
    {
        PlayerProfile GetPlayer(long id);

        void CreatePlayer(PlayerProfile profile);
    }
}