; scripts for automating picture exports from WinAGI

counter := 1
repeat := 80
export_priority := false
scale := 1

sc029 & r::
  showToolTip("reloading")
  sleep 300
  reload
  return

ShowToolTip(toolTipText, delay = 2000) {
  tooltip, %toolTipText%
}

;   script for exporting priority data
; 1. o  pen game with WinAGI
; 2. select PICTURES in the drop down
; 3. export one of the pictures to fix the desired output directory
; 4. focus on the first room to export, set %counter% to match the room number
; 5. important - the mouse cursor must also hover above the picture list
; 6. set %repeat% to the number of rooms to export
;   (for this to be consistent their numbering following the one in focus must be in
;    ascending and consecutive order in the WinAGI list)
sc029 & 1::
  loop %repeat% {
    send ^e
    sleep 500
    send {right}
    sleep 100
    send {tab}
    if (export_priority) {
      sleep 100
      send {down}
    }
    sleep 100
    send {tab}
    sleep 100
    send %scale%
    sleep 100
    send {del}
    sleep 100
    send {tab}
    sleep 50
    send {tab}
    sleep 50
    send {tab}
    sleep 50
    send {return}
    sleep 200
    send room%counter%
    counter := counter + 1
    sleep 200
    send {return}
    sleep 2000
    send {return}
    sleep 200
    mouseclick
    sleep 200
    send {down}
    sleep 300
  }
  return
