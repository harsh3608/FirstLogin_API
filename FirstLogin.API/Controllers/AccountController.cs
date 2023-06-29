using FirstLogin.Core.DTO;
using FirstLogin.Core.Identity;
using FirstLogin.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstLogin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route(("RegisterUser"))]
        public async Task<ResponseDTO<AuthenticationResponse>> RegisterUser(RegisterDTO registerDTO)
        //public async Task<ActionResult<ApplicationUser>> RegisterUser(RegisterDTO registerDTO)
        {
            ResponseDTO<AuthenticationResponse> response = new ResponseDTO<AuthenticationResponse>();
            

            //Validation
            if (ModelState.IsValid == false)
            {
                //string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                //return Problem(errorMessage);

                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Response = null;
                response.Message = "One or more validation errors occured.";

                return response;

            }


            //Create user
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                Gender = registerDTO.Gender,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                var authenticationResponse = _jwtService.CreateJwtToken(user);
                await _userManager.UpdateAsync(user);

                //return Ok(authenticationResponse);

                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Response = authenticationResponse;
                response.Message = "User registered and signed-in successfully.";

                return response;
            }
            else
            {
                //string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); //error1 | error2
                //return Problem(errorMessage);

                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Response = null;
                response.Message = "User registration failed.";

                return response;
            }
        }


        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }


        [HttpPost]
        [Route("Login")]
        //public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
        public async Task<ResponseDTO<AuthenticationResponse>> PostLogin(LoginDTO loginDTO)
        {
            ResponseDTO<AuthenticationResponse> response = new ResponseDTO<AuthenticationResponse>();
            int StatusCode = 0;
            bool isSuccess = false;
            string Response = null;
            string Message = "";

            //Validation
            if (ModelState.IsValid == false)
            {
                //string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                //return Problem(errorMessage);

                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Response = null;
                response.Message = "One or more validation errors occured.";

                return response;
            }


            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

                if (user == null)
                {
                    response.StatusCode = 404;
                    response.IsSuccess = false;
                    response.Response = null;
                    response.Message = "User not found.";

                    return response;
                }

                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                var authenticationResponse = _jwtService.CreateJwtToken(user);
             
                await _userManager.UpdateAsync(user);


                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Response = authenticationResponse;
                response.Message = "User signed-in successfully.";

                return response;
            }
            else
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Response = null;
                response.Message = "Wrong Credentials.";

                return response;
            }
        }
    }
}
