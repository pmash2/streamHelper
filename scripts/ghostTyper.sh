#!/bin/sh

case ${1} in
	cs)
		codeFile="..\assets\csharpCode.cs"
		lang=cs
		;;
	js)
		codeFile="..\assets\reactCode.js"
		lang=javascript
		;;
	*)
		codeFile="..\assets\csharpCode.cs"
		lang=cs
		;;
esac

if [ ! -f "${codeFile}" ]; then
  echo "[ERROR] Code file ${codeFile} does not exist!"
  exit 1
fi

"C:\Program Files (x86)\Notepad++\notepad++.exe" -qf${codeFile} -qSpeed2 -l${lang} -multiInst -nosession -notabbar
