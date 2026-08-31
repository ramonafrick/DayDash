using System.Buffers.Binary;

namespace DayDash.Modules.Reminder.Application.Services;

/// <summary>
/// Deterministic notification ids. The same entity maps to the same int on every call and
/// across process restarts - unlike <see cref="object.GetHashCode"/>, which is randomised
/// per run and would orphan previously scheduled notifications after a restart.
/// </summary>
public static class NotificationIds
{
    /// <summary>Fixed id for the single daily study reminder.</summary>
    public const int DailyStudyReminder = 1;

    private const uint ExamSalt = 0x4D_41_58_45;  // "EXAM"
    private const uint EventSalt = 0x54_4E_56_45; // "EVNT"

    public static int ForExam(Guid examId) => FromGuid(examId, ExamSalt);

    public static int ForEvent(Guid eventId) => FromGuid(eventId, EventSalt);

    private static int FromGuid(Guid id, uint salt)
    {
        Span<byte> bytes = stackalloc byte[16];
        id.TryWriteBytes(bytes);
        var raw = BinaryPrimitives.ReadUInt32LittleEndian(bytes) ^ salt;
        var result = (int)(raw & 0x7FFF_FFFF);
        return result < 100 ? result + 100 : result; // stay clear of the fixed low ids
    }
}
