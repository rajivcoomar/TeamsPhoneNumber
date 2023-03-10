$keyVaultName = "azdeusbaseappvault"
$clientName = "ClientSecret"
$sClientValue = "_Lk8Q~eJyw6tVpoz_BQW0yUOVSzttDv7s4k6xdgr"

$TwilioAccountSIDName = "CustomTwilioAccountSID"
$sTwilioAccountSIDValue = "ACfe63e966f04e0c8d65f0c77652704006"

$TwilioAuthTokenName = "CustomTwilioAuthToken"
$sTwilioAuthTokenValue = "b947d7c4816d5493fededfb4c2940dcc"

$subscriptioID="e3918976-5d63-4545-aa2e-62327ef59d28"

$resourceGroupName = "rg-eus-comm-dev-001"

Update-AzConfig -DefaultSubscriptionForLogin $subscriptioID
Connect-AzAccount

# Convert the secret value to a secure string
$secureClientValue = ConvertTo-SecureString -String $sClientValue -AsPlainText -Force
$secureTwilioAccountSIDValue = ConvertTo-SecureString -String $sTwilioAccountSIDValue -AsPlainText -Force
$secureTwilioAuthTokenValue = ConvertTo-SecureString -String $sTwilioAuthTokenValue -AsPlainText -Force

# Create a new Key Vault
$keyVault = Get-AzKeyVault -ResourceGroupName $resourceGroupName -Name $keyVaultName 

# Add a new secret to the Key Vault
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name $clientName -SecretValue $secureClientValue
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name $TwilioAccountSIDName -SecretValue $secureTwilioAccountSIDValue
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name $TwilioAuthTokenName -SecretValue $secureTwilioAuthTokenValue