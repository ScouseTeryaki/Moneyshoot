import urllib.request
import re
import csv


def main():
    finals = check_for_csv('finals.csv')
    # if not finals:
    #     print('Collecting Event Finals...')
    #     url = 'https://www.hltv.org/results?stars=5'
    #     events_list = get_webpage(url)
    #     matchers = ['<a href="/matches/']
    #     finals = get_url_from_html(events_list, matchers, 'href="', '"')
    #     dump_to_file('finals.csv', finals)
    #     events = remove_dupe_events('finals.csv')
    #     dump_to_file('finals.csv', events)
    length = len(finals)
    quarter_index = length//4
    first_quarter = finals[:quarter_index]
    events = []
    for event in first_quarter:
        final_match_url = event[0]
        final_match_page = get_webpage(final_match_url)
        event_name_list = event[1].split("-")
        event_name = "-".join(event_name_list[-3:])
        print(event_name)
        matcher = r'<a href="/events/\d{4}/'
        events = get_url_from_html(final_match_page, matcher, 'href="', '/')
    dump_to_file('events.csv', events)
    print(events)


def remove_dupe_events(filename):
    with open(filename, mode='r+', newline='') as csvfile:
        reader = csv.reader(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
        items = list(reader)
        for i in items:
            for idx, dupe in enumerate(items):
                if i == dupe:
                    items.pop(idx)
        return items


def check_for_csv(filename):
    try:
        with open(filename, mode='r') as csvfile:
            reader = csv.reader(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            reader_list = list(reader)
            # Doesn't work atm (fix later pls)
            if reader_list:
                return reader_list
            else:
                return False
    except FileNotFoundError:
        return False


def dump_to_file(filename, data):
    with open(filename, mode='w', newline='') as csvfile:
        writer = csv.writer(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
        for item in data:
            writer.writerow(item)


# Generic webpage scraper for later processing
def get_webpage(url):
    req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 '
                                                             '(KHTML, like Gecko) Chrome/36.0.1941.0 Safari/537.36'})
    web_byte = urllib.request.urlopen(req).read()
    webpage = web_byte.decode('utf-8')
    return webpage


def get_url_from_html(html, matcher, start_pointer, end_pointer):
    matching = re.findall(matcher, html)
    urls = []
    print(matching)
    for event in matching:
        temp = []
        url_suffix = re.search(rf'(?<={start_pointer}).*?(?={end_pointer})', event).group(0)
        url = "https://www.hltv.org" + url_suffix
        text_suffix = re.search(r'(?<=\d/).*?(?=")', event).group(0)
        temp.append(url)
        temp.append(text_suffix)
        urls.append(temp)
    return urls


# Downloads CSGO series in a zip
# Contains .dem files for the series
def download_demo(url):
    opener = urllib.request.build_opener()
    opener.addheaders = [('User-Agent', 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 '
                                        '(KHTML, like Gecko) Chrome/36.0.1941.0 Safari/537.36')]
    urllib.request.install_opener(opener)

    local = 'file.zip'
    urllib.request.urlretrieve(url, local)


def unzip_demo(file_dir):
    pass


main()
