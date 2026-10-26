## RoundEndSystem

# rewritten for funky
round-end-system-shuttle-called-announcement = Evacuation telemetry alert.
                                               Emergency vessel dispatched.
                                               ETA: {$time} {$units}.
round-end-system-shuttle-already-called-announcement = Evacuation telemetry advisory. Redundant dispatch rejected: emergency vessel transit already active.
round-end-system-shuttle-auto-called-announcement = Logistics telemetry update.
                                                    Automated shift-rotation transport dispatched.
                                                    ETA: {$time} {$units}.
round-end-system-shuttle-recalled-announcement = Evacuation telemetry update.
                                                 Emergency vessel recalled.
                                                 Inbound approach aborted.
round-end-system-shuttle-sender-announcement = Station
round-end-system-round-restart-eta-announcement = Restarting the round in {$time} {$units}...

eta-units-minutes = {$amount ->
    [one] minute
    *[other] minutes
}
eta-units-seconds = {$amount ->
    [one] second
    *[other] seconds
}
