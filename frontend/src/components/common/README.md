# Icon Component

## Overview
Centralized Icon component for consistent icon rendering across the entire application. Uses Font Awesome icons for professional appearance and comprehensive icon coverage.

## Usage

### Basic Usage
```jsx
import { Icon } from '../components/common';

// Simple icon
<Icon name="home" />

// Icon with custom size
<Icon name="user" size={24} />

// Icon with additional styling
<Icon name="settings" size={32} className="custom-class" />
```

### Available Icons

#### Navigation & UI
- `speedometer2` - Dashboard (tachometer)
- `people` - Users/Customers (users)
- `file-text` - Documents/Quotes (file-text)
- `person-badge` - Employee/Profile (id-badge)
- `building` - Company/Organization (building)
- `gear` - Settings (cog)
- `list` - Menu/List (bars)
- `home` - Home (home)

#### Actions
- `person-plus-fill` - Add User (user-plus)
- `file-plus-fill` - Add Document (file-plus)
- `box-arrow-right` - Logout/Exit (sign-out-alt)
- `calendar-event` - Schedule (calendar-alt)
- `lightning-charge-fill` - Quick Actions (bolt)
- `add` - Add (plus)
- `remove` - Remove (minus)
- `edit` - Edit (edit)
- `delete` - Delete (trash)
- `save` - Save (save)
- `cancel` - Cancel (times)

#### Status & Feedback
- `arrow-up` - Increase/Growth (arrow-up)
- `arrow-down` - Decrease/Decline (arrow-down)
- `clock` - Time/Pending (clock)
- `check-circle` - Success/Complete (check-circle)
- `warning` - Warning (exclamation-triangle)
- `error` - Error (times)
- `info` - Information (info-circle)
- `success` - Success (check-circle)

#### Communication
- `bell` - Notifications (bell)
- `phone` - Phone (phone)
- `email` - Email (envelope)
- `share` - Share (share)

#### Data & Files
- `currency-dollar` - Money/Revenue (dollar-sign)
- `document` - Document (file)
- `folder` - Folder (folder)
- `image` - Image (image)
- `download` - Download (download)
- `upload` - Upload (upload)
- `attachment` - Attachment (paperclip)
- `print` - Print (print)

#### Utility
- `search` - Search (search)
- `filter` - Filter (filter)
- `sort` - Sort (sort)
- `refresh` - Refresh (sync)
- `help` - Help (question-circle)
- `location` - Location (map-marker-alt)
- `star` - Favorite (star)
- `heart` - Like (heart)
- `thumbs-up` - Approve (thumbs-up)
- `thumbs-down` - Disapprove (thumbs-down)

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `name` | `string` | **required** | The name of the icon to display |
| `size` | `number` | `20` | Size of the icon in pixels |
| `className` | `string` | `''` | Additional CSS classes |
| `style` | `object` | `{}` | Additional inline styles |
| `...props` | `any` | - | Any additional props passed to the span element |

## Examples

### Dashboard Stats
```jsx
<Icon name="people-fill" size={24} />
<Icon name="currency-dollar" size={24} />
<Icon name="file-text-fill" size={24} />
```

### Navigation Menu
```jsx
<Icon name="speedometer2" size={20} />
<Icon name="people" size={20} />
<Icon name="file-text" size={20} />
<Icon name="gear" size={20} />
```

### Action Buttons
```jsx
<Icon name="person-plus-fill" size={32} />
<Icon name="file-plus-fill" size={32} />
<Icon name="calendar-event" size={32} />
```

### Status Indicators
```jsx
<Icon name="arrow-up" size={12} className="text-success" />
<Icon name="clock" size={12} className="text-warning" />
<Icon name="check-circle" size={12} className="text-success" />
```

## Styling

The component includes default styling:
- `display: inline-flex` for proper alignment
- `alignItems: center` and `justifyContent: center` for centering
- `lineHeight: 1` to prevent spacing issues
- Responsive font-size based on the `size` prop

Additional styling can be applied via:
- `className` prop for CSS classes
- `style` prop for inline styles
- CSS targeting the `.icon` class

## Font Awesome Implementation

This component uses Font Awesome icons for professional appearance and comprehensive coverage. The implementation:

1. Uses `@fortawesome/react-fontawesome` for React integration
2. Imports specific icons from `@fortawesome/free-solid-svg-icons`
3. Maps custom icon names to Font Awesome icons
4. Provides fallback for unknown icons

### Adding New Icons

To add new icons:

1. Import the icon from Font Awesome:
```jsx
import { faNewIcon } from '@fortawesome/free-solid-svg-icons';
```

2. Add to the iconMap:
```jsx
const iconMap = {
  'new-icon': faNewIcon,
  // ... existing icons
};
```

3. Use anywhere in the app:
```jsx
<Icon name="new-icon" size={24} />
```

## Accessibility

- Font Awesome automatically handles SVG accessibility
- Includes `title` attribute for hover tooltips
- Semantic icon names for better understanding
- Console warnings for missing icons to aid development
- Fallback icon (question-circle) for unknown icons
