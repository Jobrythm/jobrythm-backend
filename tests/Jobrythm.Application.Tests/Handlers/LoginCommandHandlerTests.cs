using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Auth.Commands.Login;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();

        _handler = new LoginCommandHandler(_userManagerMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@jobrythm.com", FullName = "Test User" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(user)).Returns("access-token");
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var command = new LoginCommand("test@jobrythm.com", "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.Email.Should().Be("test@jobrythm.com");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserNotFound()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        var command = new LoginCommand("unknown@jobrythm.com", "password");

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.BadRequestException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
