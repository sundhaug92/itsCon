$psDlls = ls *.dll;
foreach($dll in $psDlls)
{
	Install-Module -ModulePath $dll;
}