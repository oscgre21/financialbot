using aChatBotApi.Controllers;
using AutoMapper;
using FinancialBot.BL.DTOs;
using FinancialBot.Core.Jwt;
using FinancialBot.Core.Repository;
using FinancialBot.Domain.Entities;
using FinancialBot.Services.Mqt;
using FinancialBot.UnitTest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Reflection;

namespace FinancialBot.Test
{
	[TestClass]
	public class AccountControllerTest
	{
		
		private readonly AccountController _controller;
		//"AAL.US", "AAPL.US", "ABC.US", "ABC.US"

		public AccountControllerTest()
		{
			var userManager = SetupSetting.userManager; 
			var mapper = SetupSetting.mapper;
			var jwt = SetupSetting.jwt;
			_controller = new AccountController(userManager,mapper,jwt);
		}


		[TestMethod] 
		public async Task Should_Be_A_Valid_User_Creation_When_StatusCodeEqual201()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "Aa123456$", Password = "Aa123456$", UserName = "oscgre" };

			var ret = await _controller.RegisterUser(user) as StatusCodeResult;
			  
			Assert.IsNotNull(ret);
			Assert.AreEqual(ret.StatusCode, 201);
		}

		[TestMethod]
		public async Task Should_Be_A_Valid_User_Creation_When_UserPassword_not_Contain_LowerCase()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "A123456$", Password = "A123456$", UserName = "oscgre" };

			var ret = (await _controller.RegisterUser(user) as BadRequestObjectResult).Value as RegisterResponseDto;
			var count = ret.Errors.Where(x=>x.Contains("lowercase")).ToList().Count();

			Assert.IsNotNull(ret);
			Assert.AreEqual((count > 0), true);
		}

		[TestMethod]
		public async Task Should_Be_A_Valid_User_Creation_When_UserPassword_not_Contain_UperCase()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "a123456$", Password = "a123456$", UserName = "oscgre" };

			var ret = (await _controller.RegisterUser(user) as BadRequestObjectResult).Value as RegisterResponseDto;
			var count = ret.Errors.Where(x => x.Contains("uppercase")).ToList().Count();

			Assert.IsNotNull(ret);
			Assert.AreEqual((count > 0), true);
		}

		[TestMethod]
		public async Task Should_Be_A_Valid_User_Creation_When_UserPassword_Not_Contain_At_Least_6_Characters()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "Aa$", Password = "Aa$", UserName = "oscgre" };

			var ret = (await _controller.RegisterUser(user) as BadRequestObjectResult).Value as RegisterResponseDto;
			var count = ret.Errors.Where(x => x.Contains("Passwords must be at least 6 characters")).ToList().Count();
	
			Assert.IsNotNull(ret);
			Assert.AreEqual((count >0), true);
		}

		[TestMethod]
		public async Task Should_Be_A_Valid_User_Creation_When_UserPassword_Not_Contain_Number()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "Aaaaaaaaaa$", Password = "Aaaaaaaaaa$", UserName = "oscgre" };

			var ret = (await _controller.RegisterUser(user) as BadRequestObjectResult).Value as RegisterResponseDto;
			var count = ret.Errors.Where(x => x.Contains("Passwords must have at least one digit")).ToList().Count();

			Assert.IsNotNull(ret);
			Assert.AreEqual((count > 0), true);
		}

		[TestMethod]
		public async Task Should_Be_A_Valid_User_Creation_When_UserPassword_Not_Contain_Symbol()
		{
			var user = new UserRegistrationDto() { ConfirmPassword = "Aa123456", Password = "Aa123456", UserName = "oscgre" };

			var ret = (await _controller.RegisterUser(user) as BadRequestObjectResult).Value as RegisterResponseDto;
			var count = ret.Errors.Where(x => x.Contains("Passwords must have at least one non alphanumeric character")).ToList().Count();

			Assert.IsNotNull(ret);
			Assert.AreEqual((count > 0), true);
		}

		 

	}
}