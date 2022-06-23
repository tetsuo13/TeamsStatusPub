<img src="src/TeamsStatusPub/Icons/logo.png" width="100" alt="Logo" />

# Teams Status Pub

[![Continuous integration](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yaml/badge.svg)](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Serve a RESTful interface from a computer running [Microsoft Teams](https://www.microsoft.com/en-us/microsoft-teams/group-chat-software/) to act as a sensor for [Home Assistant](https://www.home-assistant.io/). This sensor can then be used, for example, to toggle power to a smart lightbulb when you're in a meeting.

Teams Status Pub runs on the system tray to publish your Teams status via a web server that listens for incoming requests. When a request comes in, it will look at the Teams log file to determine your status as either "available" or "busy" and serve a JSON payload with the result.

## Motiviation and Alternatives

There are other projects available for integrating Teams with Home Assistant however their usage is largly dependent on how Teams and/or your computer is configured within the organization.

### Microsoft Graph

Your status is available via the [presence](https://docs.microsoft.com/en-us/graph/api/presence-get) API in [Microsoft Graph](https://docs.microsoft.com/en-us/graph/overview) if your organization has granted consent. If so then [PresenceLight](https://github.com/isaacrlevin/PresenceLight) may be a better alternative in this scenario. Note that hardware support for bulbs is limited.

### Directly Update Home Assistant

If the computer that Teams is installed on can reach Home Assistant on the network, then [EBOOZE/TeamsStatus](https://github.com/EBOOZ/TeamsStatus) may be worth a try. After creating the status sensors in Home Assistant then provide TeamStatus with a long-lived access token, it will determine your availability from Teams locally and reach out to Home Assistant to set the values of the sensors accordingly.

### Teams Status Pub

If you've made it this far, the above alternatives are lacking something or you're in an organization that doesn't provide consent to read presense from the Graph API or the computer Teams is installed on cannot reach Home Assistant (due to network segregation or a restricted VLAN). These were the primary motivating factors behind this project.

## Getting Started

Your computer must be running Windows 10 or later.

### Teams Status Pub

The default listen address is http://10.8.15.109:17493/

In order for Home Assistant to successfully query the computer, you will likely need to add an inbound rule to allow this application/port through the firewall.

### Home Assistant

https://www.home-assistant.io/integrations/sensor.rest/

https://www.home-assistant.io/integrations/binary_sensor.rest/

```yaml
binary_sensor:
  - platform: rest
    resource: http://IP_ADDRESS:PORT/
    name: Microsoft Teams Status
    value_template: "{{ value_raw equals 'busy' }}"

# Probably don't need this
  - platform: template
    sensors:
      teams_status:
        value_template: 
```

## License

See [LICENSE](LICENSE)