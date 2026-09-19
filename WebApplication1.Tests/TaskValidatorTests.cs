using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models.Validators;

namespace WebApplication1.Tests
{
	public class TaskValidatorTests
	{
		[Fact]
		public void IsValidTitle_WhenTitleIsValid_ReturnsTrue()
		{
			// Arrange
			var validator = new TaskValidator();

			// Act
			var result = validator.IsValidTitle("Learn Angular");

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsValidTitle_WhenTitleIsEmpty_ReturnsFalse()
		{
			// Arrange
			var validator = new TaskValidator();

			// Act
			var result = validator.IsValidTitle("");

			// Assert
			Assert.False(result);
		}

	}
}
