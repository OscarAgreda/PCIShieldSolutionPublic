using MudBlazor;
using System;
namespace PCIShield.BlazorAdmin.Components.Layout
{
    public class ThemeManagerTheme
    {
        public MudTheme Theme { get; }
        public ThemeManagerTheme(string primaryColor = "#0F766E")
        {
            Theme = new MudTheme()
            {
                PaletteLight = new PaletteLight()
                {
                    Primary = "#293E8C",
                    PrimaryContrastText = "#FFFFFF",
                    PrimaryDarken = "#1B2C6A",
                    PrimaryLighten = "#E8F1FF",

                    Secondary = "#F17B40",
                    SecondaryContrastText = "#FFFFFF",

                    Success = "#6CB93C",
                    SuccessContrastText = "#FFFFFF",

                    Info = "#0EA5E9",
                    Warning = "#F59E0B",
                    Error = "#EF4444",
                    TextPrimary = "#343030",
                    TextSecondary = "#707070",
                    TextDisabled = "#AEAEAE",
                    Background = "#FBFBFB",
                    BackgroundGray = "#F6F6F6",
                    Surface = "#FFFFFF",
                    AppbarBackground = "#FBFBFB",
                    AppbarText = "#343030",

                    DrawerBackground = "#293E8C",
                    DrawerText = "#FFFFFF",
                    DrawerIcon = "#FFFFFF",
                    LinesDefault = "#DDDDDD",
                    LinesInputs = "#DDDDDD",
                    Divider = "#AEAEAE",
                    TableLines = "#AEAEAE",
                    TableStriped = "#F6F6F6",
                    TableHover = "#E8F1FF",
                    ActionDefault = "#707070",
                    ActionDisabled = "#AEAEAE",
                    ActionDisabledBackground = "rgba(41, 62, 140, 0.12)",
                    OverlayDark = "rgba(0, 0, 0, 0.45)",
                    OverlayLight = "rgba(241, 250, 255, 0.45)"
                },

                PaletteDark = new PaletteDark()
                {
                    Black = "#0B0F14",
                    White = "#FFFFFF",
                    Primary = primaryColor,
                    PrimaryContrastText = "#FFFFFF",
                    PrimaryDarken = "#0A524D",
                    PrimaryLighten = "#2DD4BF",
                    Secondary = "#FF6B3D",
                    SecondaryContrastText = "#111827",
                    SecondaryDarken = "#E05627",
                    SecondaryLighten = "#FF8A60",
                    Tertiary = "#8B5CF6",
                    TertiaryContrastText = "#FFFFFF",
                    TertiaryDarken = "#6D28D9",
                    TertiaryLighten = "#C4B5FD",
                    Info = "#38BDF8",
                    InfoContrastText = "#0B1220",
                    InfoDarken = "#0EA5E9",
                    InfoLighten = "#7DD3FC",

                    Success = "#22C55E",
                    SuccessContrastText = "#0B1220",
                    SuccessDarken = "#16A34A",
                    SuccessLighten = "#86EFAC",

                    Warning = "#F59E0B",
                    WarningContrastText = "#0B1220",
                    WarningDarken = "#B45309",
                    WarningLighten = "#FBBF24",

                    Error = "#F87171",
                    ErrorContrastText = "#0B1220",
                    ErrorDarken = "#EF4444",
                    ErrorLighten = "#FCA5A5",
                    Dark = "#0E141F",
                    DarkContrastText = "#E5E7EB",
                    DarkDarken = "#0A0F17",
                    DarkLighten = "#142033",
                    TextPrimary = "#E5E7EB",
                    TextSecondary = "#A7B1C2",
                    TextDisabled = "#5B6474",
                    ActionDefault = "#A7B1C2",
                    ActionDisabled = "#3C4656",
                    ActionDisabledBackground = "rgba(255,255,255,0.12)",
                    Background = "#0B1220",
                    BackgroundGray = "#08101B",
                    Surface = "#101826",
                    AppbarBackground = "#0E766E",
                    AppbarText = "#FFFFFF",
                    DrawerBackground = "#0B1220",
                    DrawerText = "#E5E7EB",
                    DrawerIcon = "#A7B1C2",
                    LinesDefault = "#1E293B",
                    LinesInputs = "#1E293B",
                    TableLines = "#1E293B",
                    TableStriped = "#0E1726",
                    TableHover = "#132033",
                    Divider = "#1F2A3C",
                    DividerLight = "rgba(255,255,255,0.10)",
                    Skeleton = "rgba(255,255,255,0.11)",
                    GrayDefault = "#A7B1C2",
                    GrayLight = "#5B6474",
                    GrayLighter = "#3C4656",
                    GrayDark = "#C8D0DC",
                    GrayDarker = "#E1E6ED",
                    HoverOpacity = 0.08,
                    RippleOpacity = 0.12,
                    RippleOpacitySecondary = 0.22,
                    OverlayDark = "rgba(0,0,0,0.45)",
                    OverlayLight = "rgba(8, 16, 27, 0.45)"
                },
                LayoutProperties = new LayoutProperties()
                {
                    DefaultBorderRadius = "7px",
                    DrawerWidthLeft = "194px",
                    DrawerWidthRight = "300px",
                    AppbarHeight = "76.45px",
                    DrawerMiniWidthLeft = "63px",
                    DrawerMiniWidthRight = "63px"
                },
                Shadows = new Shadow
                {
                    Elevation = new string[] {
                        "none",
                        "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)",
                        "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
                        "0px 3px 3px -2px rgba(0,0,0,0.2),0px 3px 4px 0px rgba(0,0,0,0.14),0px 1px 8px 0px rgba(0,0,0,0.12)",
                        "0px 2px 4px -1px rgba(0,0,0,0.2),0px 4px 5px 0px rgba(0,0,0,0.14),0px 1px 10px 0px rgba(0,0,0,0.12)",
                        "0px 3px 5px -1px rgba(0,0,0,0.2),0px 5px 8px 0px rgba(0,0,0,0.14),0px 1px 14px 0px rgba(0,0,0,0.12)",
                        "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
                        "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
                        "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
                        "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
                        "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
                        "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
                        "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
                        "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
                        "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
                        "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
                        "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
                        "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
                        "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
                        "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
                        "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
                        "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
                        "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
                        "0px 11px 14px -7px rgba(0,0,0,0.2),0px 23px 36px 3px rgba(0,0,0,0.14),0px 9px 44px 8px rgba(0,0,0,0.12)",
                        "0px 11px 15px -7px rgba(0,0,0,0.2),0px 24px 38px 3px rgba(0,0,0,0.14),0px 9px 46px 8px rgba(0,0,0,0.12)",
                        "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)"
                    }
                },
                Typography = new Typography()
                {
                    Default = new DefaultTypography()
                    {
                        FontFamily = new[] { "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                        FontSize = "12px",
                        FontWeight = "400",
                        LineHeight = "1.22",
                        LetterSpacing = "0.01071em"
                    },
                    H1 = new H1Typography()
                    {
                        FontSize = "24px",
                        FontWeight = "700"
                    },
                    H2 = new H2Typography()
                    {
                        FontSize = "16px",
                        FontWeight = "600"
                    },
                    H3 = new H3Typography()
                    {
                        FontSize = "16px",
                        FontWeight = "600"
                    },
                    H4 = new H4Typography()
                    {
                        FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                        FontSize = "2.125rem",
                        FontWeight = "400",
                        LineHeight = "1.235",
                        LetterSpacing = ".00735em"
                    },
                    H5 = new H5Typography()
                    {
                        FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                        FontSize = "1.5rem",
                        FontWeight = "400",
                        LineHeight = "1.334",
                        LetterSpacing = "0"
                    },
                    H6 = new H6Typography()
                    {
                        FontSize = "16px",
                        FontWeight = "600"
                    },
                    Subtitle1 = new Subtitle1Typography()
                    {
                        FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                        FontSize = "1rem",
                        FontWeight = "400",
                        LineHeight = "1.75",
                        LetterSpacing = ".00938em"
                    },
                    Subtitle2 = new Subtitle2Typography()
                    {
                        FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                        FontSize = ".875rem",
                        FontWeight = "500",
                        LineHeight = "1.57",
                        LetterSpacing = ".00714em"
                    },
                    Body1 = new Body1Typography()
                    {
                        FontSize = "12px",
                        FontWeight = "400",
                        LineHeight = "1.22"
                    },
                    Body2 = new Body2Typography()
                    {
                        FontSize = "12px",
                        FontWeight = "400",
                        LineHeight = "1.22"
                    },
                    Button = new ButtonTypography()
                    {
                        FontSize = "12px",
                        FontWeight = "700",
                        TextTransform = "none",
                        LineHeight = "1.22"
                    },
                    Caption = new CaptionTypography()
                    {
                        FontSize = "10px",
                        FontWeight = "400"
                    },
                    Overline = new OverlineTypography()
                    {
                        FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                        FontSize = ".75rem",
                        FontWeight = "400",
                        LineHeight = "2.66",
                        LetterSpacing = ".08333em"
                    }
                },
                ZIndex = new ZIndex()
                {
                    Drawer = 1100,
                    AppBar = 1200,
                    Dialog = 1300,
                    Popover = 1400,
                    Snackbar = 1500,
                    Tooltip = 1600
                }
            };
        }
    }
}