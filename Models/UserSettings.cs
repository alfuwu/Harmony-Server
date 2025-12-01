using Server.Models.Enums.Settings;

namespace Server.Models;
public class UserSettings {
    public long UserId { get; set; }
    public User? User { get; set; } = null;

    // APPEARANCE
    public Theme Theme { get; set; } = Theme.System;
    public AppIcon AppIcon { get; set; } = AppIcon.Default;
    public IconDisplayType AppIconDisplayType { get; set; } = IconDisplayType.Circle;
    public IconDisplayType ServerIconDisplayType { get; set; } = IconDisplayType.Rounded;
    public UserIconDisplayType AvatarDisplayType { get; set; } = UserIconDisplayType.UserPreference;
    public IconDisplayType SelfAvatarDisplayType { get; set; } = IconDisplayType.Circle;
    public NameHoverBehavior NameHoverBehavior { get; set; } = NameHoverBehavior.ShowHandle;
    public NameFontDisplayType NameFontDisplayType { get; set; } = NameFontDisplayType.Everyone;
    public bool CompactMode { get; set; } = false;
    public bool ApplySaturationToRoleColors { get; set; } = false;
    public bool AlwaysUnderlineLinks { get; set; } = false;
    public RoleColor RoleColorSettings { get; set; } = RoleColor.ShowInNames;
    public bool AlwaysExpandRoles { get; set; } = false; // always expand roles when opening a user profile
    public bool ShowRoleIcons { get; set; } = true;
    public bool ShowOwnerCrown { get; set; } = true;
    
    // ACCESSIBILITY
    public bool ReduceMotion { get; set; } = false;
    public bool HighContrastMode { get; set; } = false;
    public float Saturation { get; set; } = 1.0f; // 0.0 - 2.0
    public float TextSize { get; set; } = 1.0f; // 0.5 - 2.0
    public AnimateContext StickerAnimate { get; set; }
    public AnimateContext EmojiAnimate { get; set; }
    public AnimateContext GifAnimate { get; set; }
    public AnimateContext ServerAnimate { get; set; }
    public AnimateContext ChannelAnimate { get; set; }
    public AnimateContext AvatarAnimate { get; set; }
    public AnimateContext GlowRoleAnimate { get; set; }
    public bool DyslexiaFont { get; set; } = false;
    public bool TTS { get; set; }
    public float TTSSpeed { get; set; } = 1.0f;
    public bool ShowSendMessageButton { get; set; } = true;

    // VOICE & VIDEO
    public float InputVolume { get; set; } = 1.0f; // 0.0 - 2.0
    public float OutputVolume { get; set; } = 1.0f; // 0.0 - 2.0
    public VoiceInputMode VoiceInputMode { get; set; } = VoiceInputMode.VoiceActivity;
    public float InputSensitivity { get; set; } = -60.0f; // -100.0 - 100.0 (dB)
    public bool EchoCancellation { get; set; } = true;
    public bool NoiseSuppression { get; set; } = true;
    public bool AutomaticGainControl { get; set; } = true;

    // MESSAGES
    public bool ShowMessageTimestamps { get; set; } = true;
    public bool ShowImagesFromLinks { get; set; } = true;
    public bool ShowImagesUploadedToHarmony { get; set; } = true;
    public bool ShowVideosFromLinks { get; set; } = true;
    public bool ShowVideosUploadedToHarmony { get; set; } = true;
    public bool HideLinkWhenPreviewing { get; set; } = true;
    public bool ShowWebEmbeds { get; set; } = true;
    public bool ShowReactions { get; set; } = true;
    public bool ShowReactionCount { get; set; } = true;
    public bool ShowUsersWhoReacted { get; set; } = true;
    public bool ConvertEmoticonsToEmoji { get; set; } = false;
    public SpoilerContext ShowSpoilers { get; set; } = SpoilerContext.OnClick;
    public SpoilerContext ShowSpoilersFromFriends { get; set; } = SpoilerContext.OnClick;
    public bool ShowMentionSuggestions { get; set; } = true;
    public bool HighlightMentions { get; set; } = true;
    public bool PreviewMarkdown { get; set; } = true;
    public bool SendMessagesWithCtrlEnter { get; set; } = false;

    // PRIVACY & SAFETY

    // DEV MODE
    public bool DeveloperMode { get; set; } = false;
}
