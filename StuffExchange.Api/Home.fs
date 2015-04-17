module StuffExchange.Api.Home

open Nancy

type HomeModule() as x =
    inherit NancyModule("/")

    do x.Get.["/"] <- fun _ -> 
        box "We're hard at work getting StuffExchange ready! Check back soon!"