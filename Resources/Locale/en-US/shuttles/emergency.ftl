# Commands
## Delay shuttle round end
cmd-delayroundend-desc = Stops the timer that ends the round when the emergency shuttle exits hyperspace.
cmd-delayroundend-help = Usage: delayroundend
emergency-shuttle-command-round-yes = Round delayed.
emergency-shuttle-command-round-no = Unable to delay round end.

## Dock emergency shuttle
cmd-dockemergencyshuttle-desc = Calls the emergency shuttle and docks it to the station... if it can.
cmd-dockemergencyshuttle-help = Usage: dockemergencyshuttle

## Launch emergency shuttle
cmd-launchemergencyshuttle-desc = Early launches the emergency shuttle if possible.
cmd-launchemergencyshuttle-help = Usage: launchemergencyshuttle

# Emergency shuttle
# rewritten for funky
emergency-shuttle-left = Evacuation telemetry update.
                         Emergency vessel has departed station perimeter.
                         Transit time:
                         { $transitTime } seconds.
emergency-shuttle-launch-time = Evacuation alert.
                                Emergency vessel launch in { $consoleAccumulator } seconds.
emergency-shuttle-docked = Evacuation telemetry alert.
                           Emergency vessel docked { $direction } of station perimeter, { $location }.
                           Departure in { $time } seconds.{ $extended }
emergency-shuttle-good-luck = Navigational tracking failure.
                              Evacuation vessel failed to acquire station coordinates.
                              Recovery operations aborted.
emergency-shuttle-nearby = Navigational warning.
                           Docking port acquisition failed. Evacuation vessel translated to open space: { $direction }, { $location }.
                           Departure in { $time } seconds.{ $extended }
emergency-shuttle-extended = {" "}Departure countdown extended; auxiliary hold window engaged.

# Emergency shuttle console popup / announcement
emergency-shuttle-console-no-early-launches = Early launch is disabled
emergency-shuttle-console-auth-left = {$remaining} authorizations needed until shuttle is launched early.
emergency-shuttle-console-auth-revoked = Early launch authorization revoked, {$remaining} authorizations needed.
emergency-shuttle-console-denied = Access denied

# UI
emergency-shuttle-console-window-title = Emergency Shuttle Console
emergency-shuttle-ui-engines = ENGINES:
emergency-shuttle-ui-idle = Idle
emergency-shuttle-ui-repeal-all = Repeal All
emergency-shuttle-ui-early-authorize = Early Launch Authorization
emergency-shuttle-ui-authorize = AUTHORIZE
emergency-shuttle-ui-repeal = REPEAL
emergency-shuttle-ui-authorizations = Authorizations
emergency-shuttle-ui-remaining = Remaining: {$remaining}

# Map Misc.
map-name-centcomm = Central Command
map-name-terminal = Arrivals Terminal
