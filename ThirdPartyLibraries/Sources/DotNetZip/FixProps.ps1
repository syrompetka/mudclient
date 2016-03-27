# FixProps.ps1
#
# Set a few attributes in all the AssemblyInfo.cs files in any subdirectory.
# 
# last saved Time-stamp: <Wednesday, April 23, 2008  11:52:54  (by dinoch)>
#


function Update-Props
{
  Param ([string]$Version)
  $NewCopyright = 'AssemblyCopyright("Copyright © Dino Chiesa 2007, 2008")';
  $NewCompany =  'AssemblyCompany("Microsoft")';
  $NewProduct= 'AssemblyProduct("ZipLibrary")';

  foreach ($o in $input) 
  {
    $NewFile = [System.IO.Path]::Combine($o.DirectoryName, "AssemblyInfo.cs"); 
    Write-output $o.FullName
    #  write-output $o | Get-Member
    get-content $o.FullName | 
       %{$_ -replace 'AssemblyCopyright\(".+"\)', $NewCopyright } |
       %{$_ -replace 'AssemblyProduct\(".+"\)', $NewProduct } |
       %{$_ -replace 'AssemblyCompany\(".+"\)', $NewCompany } > tmp.cs

    move-item tmp.cs $NewFile -force
  }

}


get-childitem -recurse |? {$_.Name -eq "AssemblyInfo.cs"} | Update-Props

