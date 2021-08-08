import urllib.request
import re
import csv
import zipfile


def main():
    finals = save_event_finals()
    events = save_events(finals)
    save_event_matches(events)
    # Save demo download url
    # for demo in match demos download each demo
    # Name each demo the face-up then the map name
    # Store demo in a directory named the face-up
    # Store directory in another directory named the event name
    # e.g -iem-cologne-2021
    #       -faze-vs-navi
    #           -faze-vs-navi-inferno.dem


def save_event_matches(events):
    pass


def save_events(finals):
    events = check_for_csv('events.csv')
    final_length = len(finals)
    if not events:
        events = []
        print('No events saved!')
        limiter = int(input('How many events do you want to fetch?'))
        partial_events = finals[:limiter]
        dump_events(partial_events, events)
    else:
        events_length = len(events)
        left_events = final_length - events_length
        if left_events != 0:
            print(str(events_length) + ' events saved!')
            print(str(left_events) + ' events left!')
            limiter = int(input('How many events do you want to fetch?'))
            limiter += events_length
            partial_events = finals[events_length:limiter]
            dump_events(partial_events, events)
        else:
            print('All events fetched!')
    return events


def dump_events(partial_events, events):
    new_events = []
    for event in partial_events:
        final_match_url = event[0]
        final_match_page = get_webpage(final_match_url)
        start_pointer = '<a href="/events/'
        end_pointer = f'/.*?" title="{event[1]}"'
        middle_pointer = r'\d\d\d\d'
        prefix_url = 'https://www.hltv.org/results?event='
        event = get_url_from_html(final_match_page, start_pointer, end_pointer, middle_pointer, prefix_url)
        new_events.append(event)
    events += new_events
    dump_to_file('events.csv', events)


def save_event_finals():
    events = check_for_csv('finals.csv')
    if not events:
        print('Collecting Event Finals...')
        url = 'https://www.hltv.org/results?stars=5'
        events_list = get_webpage(url)
        start_pointer = '<a href="'
        end_pointer = '" class="a-reset">'
        middle_pointer = '/matches/'
        prefix_url = 'https://www.hltv.org'
        final_urls = get_url_from_html(events_list, start_pointer, end_pointer, middle_pointer, prefix_url)
        event_names = get_event_names_from_html(events_list, '<span class="event-name">', '<')
        finals = []
        for idx, final in enumerate(final_urls):
            tmp = [final, event_names[idx]]
            finals.append(tmp)
        events = remove_dupe_events(finals)
        dump_to_file('finals.csv', events)
    return events


# Remove items with the same event name
def remove_dupe_events(event_list):
    for i in event_list:
        for idx, dupe in enumerate(event_list):
            if i == dupe:
                event_list.pop(idx)
    return event_list


# Checks for data within a csv file
# If no data exists returns False
# If file doesn't exist returns False
def check_for_csv(filename):
    try:
        with open(filename, mode='r') as csvfile:
            reader = csv.reader(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            reader_list = list(reader)
            if reader_list:
                return reader_list
            else:
                return False
    except FileNotFoundError:
        return False


# Dumps a list into a csv file
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


# Gets a url from a html element using pointers
def get_url_from_html(html, start_pointer, end_pointer, middle_pointer, prefix_url):
    matching = re.findall(rf'(?<={start_pointer}){middle_pointer}.*?(?={end_pointer})', html)
    urls = []
    print(matching)
    for event in matching:
        url = prefix_url + event
        urls.append(url)
    return urls


# Collects event names to use to remove duplicate events
def get_event_names_from_html(html, start_pointer, end_pointer):
    matching = re.findall(rf'(?<={start_pointer}).*?(?={end_pointer})', html)
    return matching


# Downloads CSGO series in a zip
# Contains .dem files for the series
def download_demo(url):
    opener = urllib.request.build_opener()
    opener.addheaders = [('User-Agent', 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 '
                                        '(KHTML, like Gecko) Chrome/36.0.1941.0 Safari/537.36')]
    urllib.request.install_opener(opener)

    local = 'file.zip'
    urllib.request.urlretrieve(url, local)


def unzip_demo(file_dir, target_dir):
    with zipfile.ZipFile(file_dir, 'r') as zip_ref:
        zip_ref.extractall(target_dir)


main()
