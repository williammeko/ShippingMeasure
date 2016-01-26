
# 1. run test case "ShippingMeasure.Db.Tests.TankCapacityImporterTests.Import"
#    this cause imported tank capacity data saved into Data.mdb
$vsTest = ""
dir Env:\VS*COMNTOOLS | sort -Property Name -Descending | % {
    $path = "$($_.Value)\..\IDE\CommonExtensions\Microsoft\TestWindow\Vstest.console.exe"
    if (Test-Path $path) {
        $vsTest = Resolve-Path $path
    }
}
$testCaseFilter = "FullyQualifiedName~ShippingMeasure.Db.Tests.TankDbTests.SavePipes
| FullyQualifiedName~ShippingMeasure.Db.Tests.TankCapacityImporterTests.Import
| FullyQualifiedName~ShippingMeasure.Db.Tests.ReceiptDbTests.AddReceipt
| FullyQualifiedName~ShippingMeasure.Db.Tests.ReceiptDbTests.AddKindOfGoods
| FullyQualifiedName~ShippingMeasure.Db.Tests.UserTests.Authorize"
. $vsTest "..\ShippingMeasure.Db.Tests\bin\Debug\ShippingMeasure.Db.Tests.dll" /TestCaseFilter:$testCaseFilter

# 2. copy Data.mdb
copy -Path "..\ShippingMeasure.Db.Tests\bin\Debug\Data\Data.mdb" -Destination ".\bin\Debug\Data.mdb"




