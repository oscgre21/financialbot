using FinancialBot.Core.Jwt;
using AutoMapper;
using FinancialBot.BL.DTOs;
using FinancialBot.Core.Repository;
using FinancialBot.Domain.Entities;
using FinancialBot.Services.Mqt;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Reflection;

namespace FinancialBot.Test
{
	[TestClass]
	public class StockBotServicesTest
	{
		private readonly IMqtBot stock;
		private readonly UserManager<Users> _userManager;
		private readonly IMapper _mapper;
		private readonly JwtClaim _jwt;
		//"AAL.US", "AAPL.US", "ABC.US", "ABC.US"

		public StockBotServicesTest()
		{
			stock = new StockBotServices();
		}


		[TestMethod]
		public void Should_BeValid_When_Stock_Command_match_With_the_Regex()
		{
			var symbol = "AAL.US";
			var command = $"/stock={symbol}";
			MethodInfo method = stock.GetType().GetMethod("GetStockCode", BindingFlags.NonPublic | BindingFlags.Instance);
			object[] parms = new object[1] { command };
			var stockResult= method.Invoke(stock, parms); 

			Assert.IsNotNull(stockResult);
			Assert.AreEqual(stockResult, symbol);
		}

	 

		[TestMethod]
		public void Should_BeInValid_When_Stock_Is_Not_a_valid_Command()
		{
			var symbol = "AAL.US";
			var command = $"/ff={symbol}";
			MethodInfo method = stock.GetType().GetMethod("GetStockCode", BindingFlags.NonPublic | BindingFlags.Instance);
			object[] parms = new object[1] { command };
			var stockResult = method.Invoke(stock, parms);

			Assert.IsNotNull(stockResult);
			Assert.AreEqual(stockResult, "");
		}

		[TestMethod]
		public void Should_BeValid_When_Send_A_Valid_Stock_Code()
		{
			var symbol = "AAL.US";
			var message = getMessage(symbol);
			var stockModel = stock.GetStockQuote(message);

			Assert.IsNotNull(stockModel);
			Assert.AreEqual(symbol, stockModel.Symbol);
		}
		[TestMethod]
		public void Should_BeInValid_When_Send_A_InValid_Stock_Code()
		{
			var symbol = "AAL.USxx";
			var message = getMessage(symbol);
			var stockModel = stock.GetStockQuote(message);

			Assert.IsNull(stockModel);
		}

		[TestMethod]
		public void Should_BeInValid_When_Send_A_Empty_Stock_Code()
		{
			var symbol = "";
			var message = getMessage(symbol);
			var stockModel = stock.GetStockQuote(message);

			Assert.IsNull(stockModel);
		}

		private UsersMessages getMessage(string symbol) {
			var message = new UsersMessages
			{
				username = "#BOT",
				sendedDateUtc = DateTime.Now,
				Message = $"/stock={symbol}",
				group = ""
			};
			return message;
		} 
	}
}