using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Repository;

namespace BusinessLogicLayer.Service
{
    public interface IUserService
    {
        Task<UserDTO> Register(RegisterDTO registerDTO);
        Task<UserDTO> Login(LoginDTO loginDTO);
        Task<UserDTO> GetById(int userId);
        Task<UserDTO> UpdateUser(int userId, UserDTO userDTO);

    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMediaRepository _mediaRepository;

        public UserService(IUserRepository userRepository, IPlayerRepository playerRepository, IMediaRepository mediaRepository)
        {
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<UserDTO> Register(RegisterDTO registerDTO)
        {
            var user = await _userRepository.Register(registerDTO.Email, registerDTO.Password, registerDTO.Role);
            if (user == null) throw new Exception("User registration failed.");

            try
            {
                var player = await _playerRepository.CreatePlayer(user.UserId, registerDTO.Username);
                if (player == null) throw new Exception("Player creation failed.");

                var media = await _mediaRepository.CreateUserImg(user.UserId, Convert.FromBase64String(registerDTO.ProfilePictureUrl));

                return new UserDTO {
                    UserId = user.UserId,
                    Email = user.Email,
                    JoinDate = user.JoinDate,
                    Role = user.Role,
                    Username = player.Username,
                    LastLogin = player.LastLogin
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<UserDTO> Login(LoginDTO loginDTO)
        {
            var user = await _userRepository.Login(loginDTO.Email, loginDTO.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            var player = await _playerRepository.GetPlayerByUserId(user.UserId);

            return new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Role = user.Role,
                Username = player.Username,
                LastLogin = player.LastLogin
            };
        }

        public async Task<UserDTO> GetById(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var player = await _playerRepository.GetPlayerByUserId(user.UserId);
            if (player == null)
            {
                throw new KeyNotFoundException("Player not found for this user");
            }

            var media = await _mediaRepository.GetByUserId(user.UserId);

            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Username = player.Username,
                LastLogin = player.LastLogin
            };
            if (media != null)
            {
                var base64 = Convert.ToBase64String(media.Data);
                userDTO.ProfilePictureUrl = $"data:{media.Type};base64,{base64}";
            }

            return userDTO;
        }

        public async Task<UserDTO> UpdateUser(int userId, UserDTO userDTO)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            user.Email = userDTO.Email;
            user.Role = userDTO.Role;
            

            var updatedUser = await _userRepository.UpdateAsync(user, userDTO.newPassword);

            var player = await _playerRepository.GetPlayerByUserId(updatedUser.UserId);
            if (player == null)
            {
                throw new KeyNotFoundException("Player not found for this user");
            }
            player.Username = userDTO.Username;
            player.LastLogin = userDTO.LastLogin;

            await _playerRepository.UpdatePlayerAsync(player);

            byte[]? imageBytes = null;
            if (!string.IsNullOrEmpty(userDTO.ProfilePictureUrl) && userDTO.ProfilePictureUrl.Contains(","))
            {
                var base64Part = userDTO.ProfilePictureUrl.Split(',')[1];
                imageBytes = Convert.FromBase64String(base64Part);
            }

            var media = await _mediaRepository.UpdateUserImg(userId, imageBytes);

            return new UserDTO
            {
                UserId = updatedUser.UserId,
                Email = updatedUser.Email,
                JoinDate = updatedUser.JoinDate,
                Role = updatedUser.Role,
                Username = player.Username,
                LastLogin = player.LastLogin,
                ProfilePictureUrl = media != null ? $"data:{media.Type};base64,{Convert.ToBase64String(media.Data)}" : null
            };
        }
    }
}
