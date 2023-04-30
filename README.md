# CloudflareDynDNS

CloudflareDynDNS is a .NET hosted service that allows you to automatically update the DNS records of your domains hosted on Cloudflare with your current public IP address. This is useful if you have a dynamic IP address assigned by your ISP and want to access your web services using a domain name.

## Features

- Supports multiple DNS zones (one per domain) and subdomains
- Configurable TTL and proxy settings for each DNS zone
- Open-source project hosted on GitHub
- Docker support (Docker Compose and Kubernetes support coming soon)
- Easy configuration via config.json file
- Rich logging via Serilog

## Installation

You can install CloudflareDynDNS as a .NET hosted service on your machine or run it as a Docker container. For more details, please refer to the installation instructions on GitHub (pending).

## Usage

To use CloudflareDynDNS, you need to have a Cloudflare account and an API token with permissions to edit DNS records. You also need to know the zone ID of each domain you want to manage with CloudflareDynDNS. You can find the zone ID on the Cloudflare dashboard under the Overview tab.

To configure CloudflareDynDNS, you need to edit the config.json file with your API token, zone IDs, and other settings. Here is an example of a config.json file:

```json
{
  "authentication": {
    "apiToken": "o_hrC9mbR60Zj0UUDl6tgZc-redacted",
    "apiKey": {
      "apiKey": "2b1edbb60517ffabede94d9redacted",
      "accountEmail": "redacted@redacted.com"
    }
  },
  "zone": {
    "zoneId": "3f1c1aa9eae062dee9redacted",
    "domain": "redacted-domain.com",
    "subdomains": [
      {
        "name": "redacted-domain.com",
        "proxied": false,
        "ttl": 60
      },
      {
        "name": "ddns.redacted-domain.com",
        "proxied": false,
        "ttl": 60
      }
    ]
  },
  "refreshTimeSeconds": 60,
  "requestTimeoutSeconds": 45
}
```

- After editing the config.json file, you can start the service or run the Docker container. CloudflareDynDNS will periodically check your public IP address and update the DNS records of your domains accordingly. You can monitor the logs to see the status of each update.

## Contributing

CloudflareDynDNS is an open-source project and welcomes contributions from anyone who is interested. If you want to contribute, please fork the GitHub repository and submit a pull request with your changes. You can also report issues or suggest features on GitHub.

## License

CloudflareDynDNS is licensed under the MIT License. See the LICENSE file for more details.