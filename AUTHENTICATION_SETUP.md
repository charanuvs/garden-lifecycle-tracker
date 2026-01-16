# Authentication Setup Guide

## Google Sign-In Setup

To enable Google authentication, you need to create OAuth 2.0 credentials in the Google Cloud Console.

### Steps:

1. **Go to Google Cloud Console**
   - Visit [https://console.cloud.google.com/](https://console.cloud.google.com/)
   - Sign in with your Google account

2. **Create a New Project** (or select an existing one)
   - Click on the project dropdown at the top
   - Click "New Project"
   - Enter a name like "Garden Tracker"
   - Click "Create"

3. **Enable Google+ API**
   - In the left sidebar, go to "APIs & Services" > "Library"
   - Search for "Google+ API"
   - Click on it and press "Enable"

4. **Create OAuth 2.0 Credentials**
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth client ID"
   - If prompted, configure the OAuth consent screen:
     - Choose "External" user type
     - Fill in required fields (App name, support email, etc.)
     - Add authorized domain if needed
     - Save and continue
   
5. **Configure OAuth Client**
   - Application type: "Web application"
   - Name: "Garden Tracker Web App"
   - Authorized JavaScript origins: `http://localhost:5140` (for development)
   - Authorized redirect URIs: 
     - `http://localhost:5140/signin-google`
     - `https://yourdomain.com/signin-google` (for production)
   - Click "Create"

6. **Save Your Credentials**
   - You'll receive a Client ID and Client Secret
   - Copy these values

7. **Update appsettings.json**
   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "YOUR_CLIENT_ID_HERE.apps.googleusercontent.com",
         "ClientSecret": "YOUR_CLIENT_SECRET_HERE"
       }
     }
   }
   ```

8. **For Production Deployment**
   - Create a separate OAuth client for production
   - Use environment variables or user secrets for credentials:
     ```bash
     dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
     dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
     ```

## Apple Sign-In Setup (Future Enhancement)

Apple Sign-In requires:
1. Apple Developer Account ($99/year)
2. Registered App ID
3. Service ID for Sign in with Apple
4. Private key for authentication

The infrastructure is in place, but Apple authentication is commented out in the login view until configured.

## Security Notes

- **Never commit your ClientId and ClientSecret to version control**
- Use `appsettings.Development.json` for local development (already in .gitignore)
- Use environment variables or Azure Key Vault for production
- Set `RequireHttpsMetadata` to `true` in production

## Testing Locally

1. Configure Google OAuth as described above
2. Update `appsettings.json` or `appsettings.Development.json`
3. Run the application: `dotnet run --project GardenTracker.Web`
4. Navigate to http://localhost:5140
5. Click "Sign In" and choose "Continue with Google"
6. Complete the Google sign-in flow

## Troubleshooting

### "Error 400: redirect_uri_mismatch"
- Make sure `http://localhost:5140/signin-google` is added to Authorized redirect URIs in Google Cloud Console
- Check that the port number matches (5140)

### "Error loading external login information"
- Verify ClientId and ClientSecret are correct
- Check that Google+ API is enabled
- Ensure redirect URI matches exactly

### "Invalid Client"
- Client credentials may be incorrect
- Try creating new credentials in Google Cloud Console
