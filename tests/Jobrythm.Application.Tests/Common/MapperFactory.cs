using AutoMapper;
using Jobrythm.Application.DTOs;
using Moq;

namespace Jobrythm.Application.Tests.Common;

public static class MapperFactory
{
    public static IMapper Create()
    {
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
        var mockLoggerFactory = new Mock<Microsoft.Extensions.Logging.ILoggerFactory>();
        mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(Jobrythm.Application.IApplicationMarker).Assembly);
        }, mockLoggerFactory.Object);

        return config.CreateMapper();
    }
}
