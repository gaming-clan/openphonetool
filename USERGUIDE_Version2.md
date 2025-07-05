# OpenPhoneTool User Guide

## Introduction

OpenPhoneTool is a free, open-source Windows application for unlocking smartphones and transferring WhatsApp data between Android and iOS devices.

## Unlock Tab

- Select your device from the dropdown.
- Choose:
    - **Remove Android Screen Lock** (ADB debugging must be enabled)
    - **Fastboot Factory Reset** (bootloader/Fastboot mode required, all data lost)
    - **Attempt FRP Bypass** (only manual instructions provided for most devices)
- Follow on-screen prompts for unlocking (watch device for confirmations/reboots).

## WhatsApp Transfer Tab

- Choose your device.
- Use **Backup** and **Restore** for WhatsApp:
    - Android: uses ADB
    - iOS: uses iTunes/libimobiledevice
- For cross-platform transfer, use WhatsApp’s official migration tools.

## Settings Tab

- Change theme (Light/Dark)
- Select UI language (English, Español)
- Set backup directory
- Adjust logging level

## Logs Tab

- View logs of operations and errors
- Export logs for troubleshooting

---

## Legal & Limitations

- Android screen lock removal only works if USB debugging is enabled and allowed.
- FRP bypass is only possible for some legacy devices; manual instructions are provided.
- iOS unlocking (iCloud, Apple ID) is not feasible with open-source tools.
- WhatsApp transfer works via backup/restore only.

---

## Troubleshooting

- Ensure you're running the portable `.exe` or have the .NET 6.0+ runtime.
- If you see missing device errors, check your USB connection and drivers.
- All logs are available in the Logs tab for support.