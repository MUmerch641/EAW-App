# Quick Testing Guide for MAUI Hybrid App

## How to Use This Guide

This guide helps you systematically test the migrated MAUI app against the original Xamarin app.

---

## Step 1: Prepare Testing Environment

### Install Original Xamarin App
```bash
# Once client provides the .apk file:
adb install path/to/original-app.apk
```

### Run Both Apps
- **Xamarin App:** On physical device or emulator
- **MAUI App:** On Android emulator (currently running)

---

## Step 2: Feature Comparison Matrix

Create a spreadsheet or document with these columns:

| Feature | Xamarin Behavior | MAUI Behavior | Status | Notes |
|---------|------------------|---------------|--------|-------|
| Login - Valid Credentials | Navigates to Dashboard | ? | ⏳ | |
| Login - Invalid Credentials | Shows error message | ? | ⏳ | |
| Login - Remember Me | Persists on app restart | ? | ⏳ | |
| ... | | | | |

---

## Step 3: Test Scenarios by Module

### 🔐 Authentication Module

**Test Cases:**
1. **Login with valid username/password**
   - Expected: Navigate to Dashboard
   - Check: Token stored correctly?
   
2. **Login with invalid credentials**
   - Expected: Error message displayed
   - Check: Error message matches original app?

3. **Remember Me checkbox**
   - Expected: Auto-login on next app launch
   - Check: Token persists in FormSession?

4. **Forgot Password**
   - Expected: Password reset flow
   - Check: Email sent? OTP verification?

5. **Logout**
   - Expected: Clear session, return to login
   - Check: All cached data cleared?

---

### 📊 Dashboard Module

**Test Cases:**
1. **Dashboard loads on login**
   - Compare: Widget layout, data displayed
   - Check: API endpoint returning data?

2. **Data refresh**
   - Expected: Pull-to-refresh works
   - Check: Loading indicator appears?

3. **Quick actions/buttons**
   - Expected: Navigate to correct pages
   - Compare: Same actions available?

---

### ⏰ Attendance Module

**Test Cases:**
1. **Check-in**
   - Expected: Record time, update status
   - Check: Location tracked? (if GPS used)

2. **Check-out**
   - Expected: Calculate hours worked
   - Check: Data saved to backend?

3. **Attendance history**
   - Expected: List of past check-ins
   - Compare: Date format, sorting

---

### 🕐 Time Entry Module

**Test Cases:**
1. **Create new time entry**
   - Expected: Form with date/time pickers
   - Check: Validation rules same as Xamarin?

2. **Edit existing entry**
   - Expected: Load data, allow edit
   - Check: Save updates correctly?

3. **Delete entry**
   - Expected: Confirmation dialog, delete
   - Check: Syncs with backend?

4. **Submit time entries**
   - Expected: Batch submit for approval
   - Check: Status changes?

---

### 🏖️ Leave Module

**Test Cases:**
1. **Request new leave**
   - Expected: Form with leave types, dates, reason
   - Check: Dropdown options match?

2. **Upload attachment** (if applicable)
   - Expected: Camera or file picker
   - Check: Upload succeeds?

3. **View leave balance**
   - Expected: Shows available days
   - Compare: Calculation matches?

4. **Cancel pending leave**
   - Expected: Status changes to cancelled
   - Check: Backend updates?

---

### ⏱️ Overtime Module

**Test Cases:**
1. **Submit OT request**
   - Expected: Date, hours, reason fields
   - Check: Validation rules?

2. **Attach supporting documents**
   - Expected: Upload files
   - Check: File types allowed?

3. **Track OT status**
   - Expected: Pending/Approved/Rejected
   - Check: Real-time updates via SignalR?

---

### 🚗 Official Business Module

**Test Cases:**
1. **Create business travel request**
   - Expected: Destination, dates, purpose
   - Check: Form fields match Xamarin?

2. **Upload receipts**
   - Expected: Multiple file uploads
   - Check: Image compression?

3. **Approve/Reject (if manager role)**
   - Expected: Action buttons
   - Check: Notifications sent?

---

### ✅ Approvals Module

**Test Cases:**
1. **View pending approvals**
   - Expected: List of items awaiting action
   - Check: Filtering/sorting?

2. **Approve request**
   - Expected: Confirmation, status update
   - Check: Notifications to requester?

3. **Reject request**
   - Expected: Reason required?
   - Check: Backend updates?

4. **Bulk actions** (if available)
   - Expected: Select multiple, approve all
   - Check: Performance?

---

### 👤 Profile Module

**Test Cases:**
1. **View profile**
   - Expected: Personal info, photo
   - Compare: Fields displayed?

2. **Edit profile**
   - Expected: Update allowed fields
   - Check: Validation?

3. **Change password**
   - Expected: Old password + new password
   - Check: Security rules enforced?

4. **Upload profile photo**
   - Expected: Camera or gallery
   - Check: Image crop/resize?

---

## Step 4: Advanced Features Testing

### 🔔 Notifications

**Test Cases:**
1. **Local notifications**
   - Trigger: Approval pending
   - Expected: Notification appears
   - Check: Tap opens correct page?

2. **SignalR real-time**
   - Trigger: Another user approves your request
   - Expected: Dashboard updates instantly
   - Check: Connection stable?

---

### 📱 Platform Features

**Test Cases:**
1. **Camera access**
   - Expected: Permission requested, camera opens
   - Check: Photo captured and uploaded?

2. **File picker**
   - Expected: System file picker appears
   - Check: Selected file uploads?

3. **Network offline**
   - Expected: Cached data shown
   - Check: Sync when online?

4. **App resume/suspend**
   - Expected: State preserved
   - Check: Token still valid?

---

## Step 5: UI/UX Comparison

### Visual Elements to Compare

**Colors:**
- Primary brand color: ____________
- Accent color: ____________
- Error color: ____________
- Success color: ____________

**Typography:**
- Primary font: ____________
- Font sizes match?
- Line spacing consistent?

**Layouts:**
- Button sizes and styles
- Input field styling
- Card/list item layouts
- Spacing and padding

**Icons:**
- Same icon set used?
- Icon colors match?

---

## Step 6: Performance Testing

**Metrics to Compare:**

1. **App Launch Time**
   - Xamarin: _______ seconds
   - MAUI: _______ seconds

2. **Login Response Time**
   - Xamarin: _______ seconds
   - MAUI: _______ seconds

3. **Dashboard Load Time**
   - Xamarin: _______ seconds
   - MAUI: _______ seconds

4. **Memory Usage**
   - Xamarin: _______ MB
   - MAUI: _______ MB

5. **Battery Drain**
   - Test for 1 hour of usage
   - Compare battery % drop

---

## Step 7: Error Scenarios

**Test These:**

1. **Network timeout**
   - Enable airplane mode during API call
   - Expected: Error message, retry option

2. **Server error (500)**
   - Expected: User-friendly error

3. **Unauthorized (401)**
   - Expected: Redirect to login

4. **Invalid data submission**
   - Expected: Validation errors shown

5. **App crash recovery**
   - Expected: AppCenter logs crash

---

## Step 8: Document Findings

### Create Issue List

For each bug/difference found:

**Title:** [Module] Brief description  
**Severity:** Critical / High / Medium / Low  
**Type:** Bug / Missing Feature / UI Difference  
**Xamarin Behavior:** What original app does  
**MAUI Behavior:** What new app does  
**Steps to Reproduce:**
1. ...
2. ...
3. ...

**Expected Result:**  
**Actual Result:**  
**Screenshots:** Attach if applicable  
**Logs:** Error messages from console  

---

## Step 9: Prioritization

### Critical (Fix First)
- App crashes
- Cannot login
- Data loss
- Security issues

### High (Fix Soon)
- Major features broken
- API integration failures
- Navigation issues

### Medium (Fix After High)
- UI differences
- Missing minor features
- Performance issues

### Low (Polish Phase)
- Cosmetic issues
- Nice-to-have features
- Optimization

---

## Step 10: Track Progress

Use the todo list in GitHub Copilot to track:
- ✅ Completed fixes
- 🔄 In progress
- ⏳ Not started

---

## Tools & Commands

### Check Current Build Status
```bash
dotnet build MauiHybridApp.csproj
```

### Run on Android
```bash
dotnet build MauiHybridApp.csproj -t:Run -f net8.0-android
```

### View Android Logs
```bash
adb logcat | grep -i "MauiHybridApp"
```

### Debug in Chrome DevTools
1. Open Android emulator
2. Open Chrome: `chrome://inspect`
3. Find BlazorWebView instance
4. Click "inspect"

---

## Checklist Summary

- [ ] Install Xamarin .apk
- [ ] Test all authentication flows
- [ ] Test each module (8 modules total)
- [ ] Test notifications and real-time features
- [ ] Test file uploads and camera
- [ ] Test offline mode
- [ ] Compare UI/UX elements
- [ ] Measure performance metrics
- [ ] Test error scenarios
- [ ] Document all findings
- [ ] Prioritize fixes
- [ ] Create implementation plan

---

**Ready to Start Testing!** 🚀
