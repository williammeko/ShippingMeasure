if (![System.Diagnostics.EventLog]::SourceExists("ShippingMeasure.WinForm")) {
	Write-Host "press [ENTER] to create event source ShippingMeasure.WinForm ... " -NoNewline
	Read-Host
	New-EventLog -LogName "ShippingMeasure" -Source "ShippingMeasure.WinForm"
}

Write-Host "deploy finish, press [ENTER] to exit ... "
Read-Host

