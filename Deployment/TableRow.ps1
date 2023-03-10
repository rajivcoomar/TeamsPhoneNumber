$storageAccountName = "tucpfxonqdwuw"
$resourceGroup = "rg-eus-comm-dev-001"
$tableName = "CustomAppConfig"
$subscriptioID="e3918976-5d63-4545-aa2e-62327ef59d28"

Update-AzConfig -DefaultSubscriptionForLogin $subscriptioID
Connect-AzAccount


$storageAccount=Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storageAccountName
$ctx = $storageAccount.Context

$cloudTable = (Get-AzStorageTable -Name $tableName -Context $ctx).CloudTable

$partitionKey1 = "Setting"

Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("InitialSMSMessage_EN") -property @{"Value"="Welcome to the Del Monte Communicator.  Reply ENGLISH or SPANISH at any time in order to change your preferred language.  Reply STOP to quit receiving text messages from this service. Terms and Conditions - https://www.apple.com/legal/internet-services/itunes/ Privacy Policy - https://www.apple.com/legal/privacy/en-ww/"}

Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("InitialSMSMessage_ES") -property @{"Value"="Bienvenido al Comunicador de la Compañía Del Monte. Responda INGLÉS o ESPAÑOL en cualquier momento para cambiar su idioma preferido. Responda STOP para dejar de recibir mensajes de texto de este servicio."}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("InitialTeamsMessage_EN") -property @{"Value"="Welcome to the Del Monte Communicator.  Reply ENGLISH or SPANISH at any time in order to change your preferred language.  Reply STOP to quit receiving text messages from this service. Terms and Conditions - https://www.apple.com/legal/internet-services/itunes/ Privacy Policy - https://www.apple.com/legal/privacy/en-ww/"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("InitialTeamsMessage_ES") -property @{"Value"="Welcome to the Del Monte Communicator.  Reply ENGLISH or SPANISH at any time in order to change your preferred language.  Reply STOP to quit receiving text messages from this service. Terms and Conditions - https://www.apple.com/legal/internet-services/itunes/ Privacy Policy - https://www.apple.com/legal/privacy/en-ww/"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("LanguageChange") -property @{"Value"="You preferred language is successfully changed to "}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("LanguageChange_ES") -property @{"Value"="Su idioma preferido se cambió con éxito a"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("Opt-OutFunctionURL") -property @{"Value"="https://azdeusbaseapp-sms.azurewebsites.net/api/Opt-OutFunction?code=UeMbEgvdgy-fy-ucbSZjP5QL7JjVgS7LRAKPt3Nd8mwEAzFuFrvDUA=="}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("SendSMSFunctionURL") -property @{"Value"="https://azdeusbaseapp-sms.azurewebsites.net/api/SendSMSFunction?code=VsV5jLXMa6TdKg62vJ59qcZSBI03x8CCBbyD7Y3LFCJGAzFuzOZsOg=="}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("StopMessage") -property @{"Value"="You have been successfully unsubscribe."}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("StopMessage_ES") -property @{"Value"="Se ha dado de baja con éxito."}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("ThanksMessage") -property @{"Value"="We have noted your selection, Thanks for you reply"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("ThanksMessage_ES") -property @{"Value"="Hemos tomado nota de su selección, gracias por su respuesta"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("TwilioBroadcastNumber") -property @{"Value"="+17816307418"}
Add-AzTableRow -table $cloudTable -partitionKey $partitionKey1 -rowKey ("TwilioCallBackURL") -property @{"Value"="https://azdeusbaseapp-sms.azurewebsites.net/api/SMSCallBackURL?code=ZieXKhMckjB8CgNIcGs4h2idJAubnVxaDds5W-KN-hrOAzFuYZMfPA=="}