using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;

namespace BusinessLogicLayer.Service
{
    public interface IPlayerLibraryService
    {
    }
    public class PlayerLibraryService : IPlayerLibraryService
    {
        private readonly IPlayerLibaryRepository _playerLibaryRepository;
        public PlayerLibraryService(IPlayerLibaryRepository playerLibaryRepository)
        {
            _playerLibaryRepository = playerLibaryRepository;
        }

        //public async Task<List<PlayerLibrary>?> GetLibraryByPlayerId(int playerId)
        //{

        //}
    }
}
