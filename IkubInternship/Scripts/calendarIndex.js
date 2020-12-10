<link href="//cdnjs.cloudflare.com/ajax/libs/fullcalendar/3.4.0/fullcalendar.min.css" rel="stylesheet" />
  <link href="//cdnjs.cloudflare.com/ajax/libs/fullcalendar/3.4.0/fullcalendar.print.css" rel="stylesheet" media="print" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.47/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />

  <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.18.1/moment.min.js"></script>
  <script src="//cdnjs.cloudflare.com/ajax/libs/fullcalendar/3.4.0/fullcalendar.min.js"></script>
  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.47/js/bootstrap-datetimepicker.min.js"></script>



      var events = [];
      var selectedEvent = null;

      function FetchEventAndRenderCalendar() {
      events = [];
    $.ajax({
      type: "GET",
    url: "/HR/Event/GetEvents",
          success: function (data) {
      $.each(data, function (i, v) {
        events.push({
          eventID: v.EventId,
          title: v.Title,
          description: v.Description,
          start: moment(v.StartDate),
          end: v.FinishDate != null ? moment(v.FinishDate) : null
        });
      })

            GenerateCalender(events);
  },
          error: function (error) {
      alert('failed');
    }
  })
}

      function GenerateCalender(events) {
      $('#calender').fullCalendar('destroy');
    $('#calender').fullCalendar({
      contentHeight: 400,
    defaultDate: new Date(),
    timeFormat: 'h(:mm)a',
          header: {
      left: 'prev,next today',
    center: 'title',
    right: 'month,basicWeek,basicDay,agenda'
  },
  eventLimit: true,
  eventColor: '#378006',
  events: events,
          eventClick: function (calEvent, jsEvent, view) {
      selectedEvent = calEvent;
    $('#myModal #eventTitle').text(calEvent.title);
            var $description = $('<div />');
            $description.append($('<p />').html('<b>Start:</b>' + calEvent.start.format("DD-MMM-YYYY HH:mm a")));
            if (calEvent.end != null) {
      $description.append($('<p/>').html('<b>End:</b>' + calEvent.end.format("DD-MMM-YYYY HH:mm a")));
    }
            $description.append($('<p />').html('<b>Description:</b>' + calEvent.description));
    $('#myModal #pDetails').empty().html($description);

    $('#myModal').modal();
  },
  selectable: true,
          select: function (start, end) {
      selectedEvent = {
        eventID: 0,
        title: '',
        description: '',
        start: start,
        end: end
      };
    openAddEditForm();
    $('#calendar').fullCalendar('unselect');
  },
  editable: true,
          eventDrop: function (event) {
            var data = {
      EventID: event.eventID,
    Subject: event.title,
    Start: event.start.format('DD/MM/YYYY HH:mm A'),
    End: event.end != null ? event.end.format('DD/MM/YYYY HH:mm A') : null,
    Description: event.description
  };
  SaveEvent(data);
}
})
}

      $('#btnEdit').click(function () {
      //Open modal dialog for edit event
      openAddEditForm();
    })
      $('#btnDelete').click(function () {
        if (selectedEvent != null && confirm('Are you sure?')) {
      $.ajax({
        type: "POST",
        url: '/HR/Event/Delete',
        data: { 'eventID': selectedEvent.eventID },
        success: function (data) {
          if (data.status) {
            //Refresh the calender
            FetchEventAndRenderCalendar();
            $('#myModal').modal('hide');
          }
        },
        error: function () {
          alert('Failed');
        }
      })
    }
    })
      $('#dtp1,#dtp2').datetimepicker({
      format: 'DD/MM/YYYY HH:mm A'
  });

      function openAddEditForm() {
        if (selectedEvent != null) {
      $('#hdEventID').val(selectedEvent.eventID);
    $('#txtSubject').val(selectedEvent.title);
    $('#txtStart').val(selectedEvent.start.format('DD/MM/YYYY HH:mm A'));
    $('#txtEnd').val(selectedEvent.end != null ? selectedEvent.end.format('DD/MM/YYYY HH:mm A') : '');
    $('#txtDescription').val(selectedEvent.description);
  }
  $('#myModal').modal('hide');
  $('#myModalSave').modal();
}
      $('#btnSave').click(function () {
        //Validation/
        if ($('#txtSubject').val().trim() == "") {
      alert('Title required');
    return;
  }
        if ($('#txtStart').val().trim() == "") {
      alert('Start date required');
    return;
  }
  var startDate = moment($('#txtStart').val(), "DD/MM/YYYY HH:mm A").toDate();
  var endDate = moment($('#txtEnd').val(), "DD/MM/YYYY HH:mm A").toDate();
        if (startDate > endDate) {
      alert('Invalid end date');
    return;
  }

        var data = {
      EventId: $('#hdEventID').val(),
    Title: $('#txtSubject').val().trim(),
    StartDate: $('#txtStart').val().trim(),
    FinishDate: $('#chkIsFullDay').is(':checked') ? null : $('#txtEnd').val().trim(),
    Description: $('#txtDescription').val()
  }
  SaveEvent(data);
  // call function for submit data to the server
})
      function SaveEvent(data) {
      $.ajax({
        type: "POST",
        url: '/HR/Event/SaveOrUpdateEvent',
        data: data,
        success: function (data) {
          if (data.status) {
            //Refresh the calender
            FetchEventAndRenderCalendar();
            $('#myModalSave').modal('hide');
          }
        },
        error: function () {
          alert('Failed');
        }
      })
    }


