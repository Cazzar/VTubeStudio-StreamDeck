using System.Text.Json;
using VTubeStudio.Api;
using VTubeStudio.Api.Requests;
using Xunit;

namespace Cazzar.Deck.VTubeStudio.Tests;

public class MoveModelRequestTests
{
    // VTube Studio ignores a move that carries no timeInSeconds, which reads as the action doing nothing.
    [Fact]
    public void Time_in_seconds_is_sent_even_when_it_was_never_set()
    {
        var json = JsonSerializer.Serialize(new MoveModelRequest { Size = 10 }, VtsJson.Options);

        Assert.Contains("\"timeInSeconds\"", json);
    }

    [Fact]
    public void Untouched_axes_are_left_out_so_they_keep_their_current_value()
    {
        var json = JsonSerializer.Serialize(new MoveModelRequest { Size = 10 }, VtsJson.Options);

        Assert.DoesNotContain("positionX", json);
        Assert.DoesNotContain("rotation", json);
    }
}
