$subscriptioID="e3918976-5d63-4545-aa2e-62327ef59d28"

$resourceGroupName = "rg-eus-comm-dev-001"

Update-AzConfig -DefaultSubscriptionForLogin $subscriptioID
Connect-AzAccount

function WriteI{
    param(
        [parameter(mandatory = $true)]
        [string]$message
    )
    Write-Host $message -foregroundcolor white
}

$rg = Get-AzResource -ResourceGroupName $resourceGroupName

$tag = @{AppType='APP'; APPName='Company Communicator';BuildDate='10032023'; EnviTier='prod'; Department='IT'}


foreach($r in $rg){
	$val = $r.ResourceId
	 WriteI -message "App: $val "
   New-AzTag -ResourceId $r.ResourceId -Tag $tag -Verbose
}

#New-AZTag -ResourceId $rg.ResourceId -Tag $tag -Verbose