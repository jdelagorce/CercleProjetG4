Obtenir la liste des pépriphériques éjectables (les clés usb, disque durs usb...) et les éjecter...---------------------------------------------------------------------------------------------------
Url     : http://codes-sources.commentcamarche.net/source/53908-obtenir-la-liste-des-pepripheriques-ejectables-les-cles-usb-disque-durs-usb-et-les-ejecterAuteur  : ShareVBDate    : 06/08/2013
Licence :
=========

Ce document intitulé « Obtenir la liste des pépriphériques éjectables (les clés usb, disque durs usb...) et les éjecter... » issu de CommentCaMarche
(codes-sources.commentcamarche.net) est mis à disposition sous les termes de
la licence Creative Commons. Vous pouvez copier, modifier des copies de cette
source, dans les conditions fixées par la licence, tant que cette note
apparaît clairement.

Description :
=============

Ce code permet de lister les p&eacute;riph&eacute;riques &eacute;jectables et de
 les &eacute;jecter...
<br />Il permet aussi d'obtenir des informations sur ces
 p&eacute;riph&eacute;riques et leurs enfants (par ex : les cl&eacute;s USB)...p
our cela, il faut cliquer &agrave; droite sur la ligne du device...
<br />
<br
 />Pour plus d'informations voir :
<br />la Windows DDK sur msdn.microsoft.com 
rubrique Device Installation
<br />les fonctions SetupDiXxx : Device Installati
on functions
<br />les fonctions CM_Xxx : PnP Configuration Manager functions

<br />
<br />Pour 9x/ME : la liste des p&eacute;riph&eacute;riques &eacute;ject
ables est potentiellement incorrecte (beaucoup trop de p&eacute;riph&eacute;riqu
es) du fait d'erreur dans la prise en charge des capacit&eacute;s sous 9x/ME.
<
br /><a name='conclusion'></a><h2> Conclusion : </h2>
<br />Le code doit march
er sous 9x/ME/2000/XP. Test&eacute; sous XP/Vista/Seven et Seven x64.
<br />
<
br />N'h&eacute;sitez pas &agrave; commenter et &agrave; noter...
